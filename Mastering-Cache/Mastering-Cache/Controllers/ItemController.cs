using Mastering_Cache.Data;
using Mastering_Cache.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text.Json;

namespace Mastering_Cache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cacheRedis;
        private readonly Test2Context _context;
        private readonly string CacheKey = "invoices:all";

        public ItemController(IMemoryCache cache, IDistributedCache cacheRedis, Test2Context context)
        {
            _cache = cache;
            _context = context;
            _cacheRedis = cacheRedis;
        }

        #region "Cache Strategies"

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _context.TblItems.ToListAsync();
                return Ok(result);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetInvoices")]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                //check on cahce first
                if (_cache.TryGetValue(CacheKey, out List<TblInvoice>? invoice))
                    return Ok(invoice);
                //call item from db
                var result = await _context.TblInvoices.ToListAsync();
                //set cache options
                var options = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    .SetPriority(CacheItemPriority.High);

                //set in cache
                _cache.Set(CacheKey, result, options);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(TblInvoice invoice)
        {
            try
            {
                var checkData = await _context.TblInvoices.FirstOrDefaultAsync(a => a.TranId == invoice.TranId);
                if (checkData == null)
                {
                    return NotFound();
                }

                checkData.ItemId = invoice.ItemId;
                checkData.TranRefNo = invoice.TranRefNo;
                checkData.Quantity = invoice.Quantity;
                checkData.CreationDate = invoice.CreationDate;
                checkData.PricingListId = invoice.PricingListId;
                checkData.SalePrice = invoice.SalePrice;

                var result = await _context.SaveChangesAsync();

                if (result == 0)
                    return BadRequest("Not updated");

                _cache.Remove(CacheKey);

                return Ok("Updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("GetInvoicesByRedis")]
        public async Task<IActionResult> GetInvoicesByRedis()
        {
            try
            {
                //check on cahce first
                var checkCache = await _cacheRedis.GetStringAsync(CacheKey);
                if(checkCache is not null)
                    return Ok(JsonSerializer.Deserialize<List<TblInvoice>>(checkCache)!);
                //call item from db
                var result = await _context.TblInvoices.ToListAsync();

                //serialize
                var serializedData = System.Text.Json.JsonSerializer.Serialize(result);

                //set in cache
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                await _cacheRedis.SetStringAsync(CacheKey, serializedData, options);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateByRefis(TblInvoice invoice)
        {
            try
            {
                var checkData = await _context.TblInvoices.FirstOrDefaultAsync(a => a.TranId == invoice.TranId);
                if (checkData == null)
                {
                    return NotFound();
                }

                checkData.ItemId = invoice.ItemId;
                checkData.TranRefNo = invoice.TranRefNo;
                checkData.Quantity = invoice.Quantity;
                checkData.CreationDate = invoice.CreationDate;
                checkData.PricingListId = invoice.PricingListId;
                checkData.SalePrice = invoice.SalePrice;

                var result = await _context.SaveChangesAsync();

                if (result == 0)
                    return BadRequest("Not updated");

                _cacheRedis.Remove(CacheKey);

                return Ok("Updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetInvoicesHybrid")]
        public async Task<IActionResult> GetInvoicesHybrid()
        {
            try
            {
                //L1
                //check on cahce first
                if (_cache.TryGetValue(CacheKey, out List<TblInvoice>? invoice))
                    return Ok(invoice);

                //L2
                var checkCache = await _cacheRedis.GetStringAsync(CacheKey);
                if (checkCache is not null)
                {
                    _cache.Set(CacheKey, checkCache,new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
                    return Ok(JsonSerializer.Deserialize<List<TblInvoice>>(checkCache)!);
                }
                //call item from db
                var result = await _context.TblInvoices.ToListAsync();

                //set in cache L1
                _cache.Set(CacheKey, result, new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

                //serialize
                var serializedData = System.Text.Json.JsonSerializer.Serialize(result);
                
                //set in cache L2
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                await _cacheRedis.SetStringAsync(CacheKey, serializedData, options);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion


        #region "Cache Patterns"

        [HttpGet("GetInvoices")]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                //check on cahce first
                if (_cache.TryGetValue(CacheKey, out List<TblInvoice>? invoice))
                    return Ok(invoice);
                //call item from db
                var result = await _context.TblInvoices.ToListAsync();
                //set cache options
                var options = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    .SetPriority(CacheItemPriority.High);

                //set in cache
                _cache.Set(CacheKey, result, options);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion
    }
}
