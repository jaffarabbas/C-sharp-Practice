namespace TestSwitchCase
{
    class Program
    {
        public static void func1()
        {
            Console.WriteLine("func 1");
        }
        public static void func2()
        {
            Console.WriteLine("func 2");
        }

        public static void testSwitch(int n)
        {
            int name = n;
            var body = name switch
            {
                1 => (Action)(() => func1()),
                2 => () => func2(),
                _ => () =>
                {
                    Console.WriteLine("Fuck off");
                }
            };
        }

        public void testSwitchOld()
        {
            int name = 0;
            switch (name)
            {
                case 1:
                    func1();
                    break;
                case 2:
                    func1();
                    break;
                default:
                    break;
            }
        }
        static public void Main(String[] args)
        {
            int name = 1;
            var body = name switch
            {
                1 => (Action)(() => func1()),
                2 => () => func2(),
                _ => () =>
                {
                    Console.WriteLine("Fuck off");
                }
            };
            body();
        }
    }
}