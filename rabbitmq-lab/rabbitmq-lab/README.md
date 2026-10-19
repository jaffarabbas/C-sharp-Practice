# RabbitMQ Lab 1: Events, Queues, Retries (.NET 10)

A small HRMS-style system to learn RabbitMQ by breaking it on purpose.

```
                        ┌──────────────────────────┐
 Manager clicks         │  hr.events  (topic)      │
 "Approve"  ──▶ HrLab.Api ─▶  key: leave.approved  │
                        └────────┬─────────┬───────┘
                                 │         │   one copy per queue
                                 ▼         ▼
        notifications.leave-approved     payroll.leave-approved
                     │                          │
          NotificationWorker            PayrollWorker (run 1, 2, 3... copies)
          (push via "FCM")              (deduct leave balance)
```

---

## 1. Why queues at all?

Today, an "Approve leave" endpoint usually does everything inside the HTTP request:

1. update the DB
2. call FCM to notify the employee
3. update payroll / leave balance
4. write audit log

If FCM takes 21 seconds (you have seen that), the manager waits 21 seconds.
If FCM is down, the whole approval fails, even though the DB part was fine.

With a queue, the API does step 1, publishes **one event** ("leave approved"), and returns in milliseconds.
Separate workers do steps 2–4, each at its own speed, each with its own retries.
If a worker is down, the messages **wait** in its queue. Nothing is lost.

---

## 2. The words you need

| Word | Meaning | In this lab |
|---|---|---|
| **Producer** | Code that sends messages | `HrLab.Api` |
| **Exchange** | Post office. Receives messages, decides which queues get a copy. Stores nothing. | `hr.events` |
| **Routing key** | Address written on the message | `leave.approved` |
| **Binding** | Rule: "queue X wants messages whose key matches Y" | `payroll.leave-approved` ← `leave.approved` |
| **Queue** | Mailbox. Stores messages until a consumer takes them. | `payroll.leave-approved` |
| **Consumer** | Code that reads a queue | `PayrollWorker` |
| **Ack** | Consumer tells the broker "done, delete it" | `BasicAckAsync` |
| **Connection / Channel** | One TCP connection per app; many light channels inside it | `RabbitConnector` |

**Exchange types**

| Type | Routes by | Use it for |
|---|---|---|
| `direct` | exact key match | simple job queues |
| `fanout` | ignores the key, sends to all bound queues | broadcast |
| `topic` | patterns: `*` = one word, `#` = zero or more words | events (what we use) |
| `headers` | message headers | rare |

Also: the **default exchange** `""` routes straight to the queue whose name equals the routing key. We use it for retry and dead queues.

**Two patterns, don't mix them up:**

- **Publish/subscribe:** different services, *different queues*, each gets **its own copy**. (Notifications and Payroll.)
- **Competing consumers:** many copies of the *same* worker on *one queue*, they **split** the messages. (Payroll ×2.)

---

## 3. Run it

You need Docker Desktop and the .NET 10 SDK.

```bash
docker compose up -d
```

Open **http://localhost:15672** (user `guest`, password `guest`). Keep this tab open, it is your X-ray.

Open **three terminals** in the lab folder:

```bash
dotnet run --project src/HrLab.NotificationWorker
dotnet run --project src/HrLab.PayrollWorker
dotnet run --project src/HrLab.Api
```

The API runs on **http://localhost:5080**. Use `requests.http` (Visual Studio/VS Code) or Postman.

> Start the workers first. On startup each worker creates its own queues and bindings.

---

## 4. Experiments (do them in order)

### E1. The safety net: publishing to nowhere
1. Stop both workers. In the UI, go to **Queues** and delete all 6 queues.
2. `POST /leaves/7/approve`

**You get `503`.** The API publishes with `mandatory: true` and **publisher confirms**.
The broker answered: "I received it, but no queue wants it." Without these two settings the message would be
silently thrown away and your API would happily return 200.
Start the workers again before continuing.

### E2. One event, two copies
`POST /leaves/7/approve` (request 1 in `requests.http`).

- Notification terminal: `PUSH sent to Ali Khan...`
- Payroll terminal: `Deducting 3 day(s)...` then `Balance updated...`

The API published **once**. In the UI open **Exchanges → hr.events → Bindings**: two queues are bound, so each got a copy.
The API does not know payroll exists. Adding an audit service later needs **zero** API changes.

### E3. Failure, retry, dead queue
`POST /leaves/26/approve` (any id divisible by 13 makes "FCM" fail).

Notification terminal:
```
Attempt 1/3 failed ... Retrying in 5s
Attempt 2/3 failed ... Retrying in 5s
Giving up ... parking in 'notifications.leave-approved.dead'
```
Payroll terminal: success. **One failing consumer does not affect the other.**

How the delay works with no extra code (see `Topology.cs`):
1. Worker calls `BasicNack(requeue: false)` → message is **dead-lettered** to `….retry`
2. `….retry` has `x-message-ttl = 5000` and no consumer → after 5 s the message expires
3. Expired messages are dead-lettered **back** to the main queue → attempt 2

In the UI open **Queues → notifications.leave-approved.dead → Get messages**. Look at the headers:
`x-death` (RabbitMQ's history of every bounce) and `x-error` (the exception we saved).

> Why not `requeue: true`? It puts the message right back at the front of the queue. While FCM is down
> your worker would retry thousands of times per second. That is called a **hot loop**.

### E4. Fix and re-drive
Pretend you fixed FCM. Change `% 13` to `% 1300` in `LeaveApprovedNotifier.cs`, restart the notification worker, then:

`POST /admin/redrive/notifications.leave-approved`

The parked message goes back into the main queue with its attempt counter reset, and the push is sent.
**This is the big win over "log the error and forget it": failed work is data you can replay.**

### E5. Scale out: competing consumers
1. Start a **second** payroll worker in a 4th terminal (`dotnet run --project src/HrLab.PayrollWorker`).
2. In the UI, **Queues → payroll.leave-approved** shows **Consumers: 2**.
3. `POST /leaves/bulk?count=50`

Both payroll terminals process different employees. Each takes 1.5 s per message, so two workers finish ~2× faster.
That is horizontal scaling, the same idea as your Auto Scaling Group, but for background work.

Now set `"PrefetchCount": 1` in the PayrollWorker `appsettings.json`, restart both, repeat. Watch **Unacked** in the UI:
- Prefetch 10: each worker grabs up to 10 at once. Fast, but one slow worker can sit on 10 messages others could do.
- Prefetch 1: perfectly fair, slightly more network chatter.

### E6. Crash in the middle
Two payroll workers running. `POST /leaves/bulk?count=10`, and while one terminal shows `Deducting...`, **close that terminal** (or kill the process).

The message it was working on was never acked, so RabbitMQ gives it to the other worker. **Nothing is lost.**

But think: what if it crashed *after* updating the balance but *before* the ack? The other worker would
deduct the leave **again**. RabbitMQ guarantees **at-least-once**, never exactly-once.
That is why `LeaveApprovedPayrollHandler` **claims** the LeaveId before working (atomic `TryAdd`), and releases
the claim if the work fails.

**Proof from testing this lab:** with 2 payroll workers, 30 leaves, and the broker restarted twice mid-traffic,
all 30 were processed, none lost. Inside one worker every redelivered copy was caught as a duplicate.
But 2 leaves were deducted twice, each time once by worker A and once by worker B, because each process
has its **own** memory. An in-memory check cannot protect a scaled-out system. The fix is a
`ProcessedMessages` table with a unique key, written in the same transaction as the balance update.
That is lesson 4.

> An earlier version of this handler did "check → work → mark done" and double-deducted even inside
> one process: two copies both passed the check before either marked it. Always **claim first**.

### E7. Restart the broker
`POST /leaves/bulk?count=20` with the payroll worker **stopped**, then `docker restart hrlab-rabbitmq`.

After restart the 20 messages are still there because the queue is **durable** and messages are **persistent**.
Start the worker: it drains them. Also watch the running notification worker: it reconnects by itself
(`AutomaticRecoveryEnabled`).

If a restart hits while a worker is mid-message you'll see
`Channel closed before ... could be settled; it will be redelivered`. That's correct: the work may be done,
but the ack never reached the broker, so the message comes back. Idempotency (E6) takes care of it.

Note: a message published **without** `Persistent = true` is lost on restart even in a durable queue.
That's why `QueueWorker.ParkAsync` forces persistence on everything it parks. (Testing caught this: a
message sent from the UI's "Publish message" form is transient by default.)

### E8. Your turn (homework)
Add an **AuditWorker** project:
1. Copy `HrLab.PayrollWorker`, rename it.
2. Queue name `audit.hr-events`, binding key **`leave.#`** (everything about leaves).
3. Add a second event `LeaveRejected` published with key `leave.rejected`.

Payroll should get only approvals; Audit should get both. You changed **no** existing code to add a new consumer.

---

## 5. Code map

| File | What to read it for |
|---|---|
| `src/HrLab.Messaging/Topology.cs` | Exchanges, queues, bindings, retry/dead-letter arguments |
| `src/HrLab.Messaging/RabbitPublisher.cs` | Publisher confirms, `mandatory`, persistent messages, re-drive |
| `src/HrLab.Messaging/QueueWorker.cs` | Prefetch, manual ack, nack → retry, park → dead, `x-death` counting |
| `src/HrLab.Messaging/RabbitConnector.cs` | One connection per app, startup retry, auto-recovery |
| `src/HrLab.NotificationWorker/LeaveApprovedNotifier.cs` | A consumer that fails sometimes |
| `src/HrLab.PayrollWorker/LeaveApprovedPayrollHandler.cs` | A consumer that must be idempotent |
| `src/HrLab.Api/Program.cs` | Publishing events from an API |

---

## 6. Rules to remember

1. **Ack after the work, never before.** Early ack + crash = lost message.
2. **Consumers must be idempotent.** At-least-once means duplicates *will* happen. Claim first (atomically), then work. Store the claim where **all** instances can see it.
3. **Never `requeue: true` in a loop.** Use a delayed retry, then a dead queue.
4. **Durable queue + persistent message + publisher confirms** = the message survives a broker restart. Missing any one breaks it.
5. **One connection per app, channels for work.** Don't open a connection per request. Don't publish from many threads on one channel.
6. **Events are past tense** (`LeaveApproved`), and the publisher doesn't know who listens.
7. **Always set prefetch.**

---

## 7. Common errors

| Error | Cause | Fix |
|---|---|---|
| `PRECONDITION_FAILED - inequivalent arg 'x-message-ttl'` | You changed `RetryDelayMs` (or other queue arguments) and the queue already exists with the old value | Delete the queue in the UI, restart the worker. (Real systems use **policies** for this.) |
| `ACCESS_REFUSED` with `guest` | `guest` only works from `localhost` | Create a real user in the UI (**Admin → Users**) for anything not on your PC |
| `503 no queue is bound` | Workers never ran, or you deleted the queues | Start the workers |
| Worker logs `RabbitMQ not reachable yet` | Container still booting | Wait, it retries automatically |

---

## 8. Why raw `RabbitMQ.Client` and not MassTransit?

- You learn what is actually happening. Frameworks hide exchanges, acks and dead-lettering; when something breaks in production you need to know them anyway.
- **MassTransit v9 (released Q1 2026) needs a paid commercial license.** v8 is still free and open source and gets security patches through 2026, but no new features. Check this before adopting it at your company.
- `RabbitMQ.Client` 7.x is the official client, free (Apache-2.0 / MPL-2.0), fully async.

Once you understand this lab, any framework (MassTransit, Wolverine, Rebus, NServiceBus) will make sense in an hour, and you will be able to judge whether it's worth it.

---

## 9. What's next

| Lesson | Topic | Why |
|---|---|---|
| **2** | Tracing across the queue | `RabbitMQ.Client` 7 emits OpenTelemetry spans. Add `.AddSource("RabbitMQ.Client.Publisher", "RabbitMQ.Client.Subscriber")` to your existing OTel setup and see API → queue → worker as **one trace**. |
| **3** | **Outbox pattern** with SQL Server | The API currently commits the DB and publishes separately. Crash between them = approved leave but no event. The outbox fixes this. |
| **4** | Idempotent consumer table | Replace the in-memory dictionary with a `ProcessedMessages` table + unique key, inside the same transaction. |
| **5** | Quorum queues, delivery limits, policies | How production RabbitMQ clusters are run. |
