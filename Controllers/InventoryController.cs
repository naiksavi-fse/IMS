using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Inventory.Entities;
using Inventory.Contracts;
using Inventory.Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        //database
        private IUnitOfWork _unitOfWork;
        //logger
        private readonly ILoggerManager _logger;

       
        public InventoryController(IUnitOfWork unitOfWork, ILoggerManager logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }

        // GET: api/<InventoryController>
        [HttpGet]
        [Authorize]
            
        public IActionResult Get()
        {
            try 
            { 
            var currentUser = HttpContext.User;

            var stocks = this._unitOfWork.Stock.FindAll();
            _logger.LogInfo($"Returned all stocks from database.");
            return Ok(stocks);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetStock action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        /// <summary>
        /// Return stock details based of stock Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/<InventoryController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = this._unitOfWork.Stock.GetStockById(id);

                if (result == null)
                {
                    _logger.LogError($"stock with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned stock with id: {id}");
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetStockById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        /// <summary>
        /// add new stock to stock table
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        // POST api/<InventoryController>
        [HttpPost]
        public IActionResult Post([FromBody] Stock stock)
        {
            try
            {
                if (stock == null)
                {
                    _logger.LogError("stock object sent from client is null.");
                    return BadRequest("stock object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid stock object sent from client.");
                    return BadRequest("Invalid model object");
                }

                this._unitOfWork.Stock.Create(stock);
                this._unitOfWork.Save();
                return Ok("Record added successfully");
            }
           
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// update stock details based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        // PUT api/<InventoryController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Stock stock)
        {
            try
            {
                if (stock == null)
                {
                    _logger.LogError("stock object sent from client is null.");
                    return BadRequest("stock object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid stock object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var entity = this._unitOfWork.Stock.GetStockById(id);
                if (entity == null)
                {

                    _logger.LogError($"stock with id: {id}, hasn't been found in db.");
                    return NotFound("No Record found");
                }

                this._unitOfWork.Stock.Update(stock);
                this._unitOfWork.Save();
                return Ok("Record Updated sucessfully...");
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateStock action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// delete stock based on id from stock table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<InventoryController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = this._unitOfWork.Stock.GetStockById(id);
                if (entity == null)
                {
                    _logger.LogError($"Stock with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                this._unitOfWork.Stock.Delete(entity);
                this._unitOfWork.Save();
                return Ok("Record deleted...");
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteStock action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


    }

}
