using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.DTOs;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers
{
    /// <summary>
    /// interact with the Book table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _book;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository book,
                               ILoggerService logger,
                               IMapper mapper)
        {
            _book = book;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Get All Books
        /// </summary>
        /// <returns>A List of Books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Attempted Call");
                var books = await _book.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location}: Successful");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location} : Attempted to get a book with id:{id}");
                var book = await _book.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"{location} failed to retrieve record with id:{id}");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location} Successfully got book with id:{id}!");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(($"{location} - {ex.Message} - {ex.InnerException}"));
            }
        }

        /// <summary>
        /// This an endpoint to create a book
        /// </summary>
        /// <param name="bookCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookCreate)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{ location }: Create Attempted");
                if (bookCreate == null)
                {
                    _logger.LogWarn($"{location}: Empty request was submitted!");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplte");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookCreate);
                var isScuccess = await _book.Create(book);
                if (!isScuccess)
                {
                    return InternalError(($"{location} - Creation failed!"));
                }
                _logger.LogInfo($"{location}: Creation was Successful!");
                return Created("Create", new { book });
            }
            catch (Exception ex)
            {
                return InternalError(($"{location} - {ex.Message} - {ex.InnerException}"));
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookUpdate)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Update Attempted on record with id: {id} ");
                if (id < 1 || bookUpdate == null || id != bookUpdate.Id)
                {
                    _logger.LogWarn($"{location}: Update failed with bad data - id: {id}");
                    return BadRequest();
                }
                var isExists = await _book.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was Incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookUpdate);
                var isSuccess = await _book.Update(book);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Update failed for record with id: {id}");
                }
                _logger.LogInfo($"{location}: Record with id: {id} successfully updated");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{location}: Delete Attempted on record with id: {id} ");
                if (id < 1)
                {
                    _logger.LogWarn($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }
                var isExists = await _book.IsExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location}: Failed to retrieve record with id: {id}");
                    return NotFound();
                }
                var book = await _book.FindById(id);
                var isSuccess = await _book.Delete(book);
                if (!isSuccess)
                {
                    return InternalError($"{location}: Delete failed for record with id: {id}");
                }
                _logger.LogInfo($"{location}: Record with id: {id} successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong,please contact the administrator!");
        }
    }
}
