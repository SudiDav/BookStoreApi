﻿using AutoMapper;
using BookStoreApi.Contracts;
using BookStoreApi.DTOs;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers
{
    /// <summary>
    /// Endpoint to interact with the Authors in the book store's Db
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _author;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public AuthorsController(IAuthorRepository author,
                                 ILoggerService logger,
                                 IMapper mapper)
        {
            _author = author;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All authors
        /// </summary>
        /// <returns>List of Authors</returns>

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                var authors = await _author.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully Got all Authors!");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(($"{ex.Message} - {ex.InnerException}"));
            }
        }
        /// <summary>
        /// Get an author by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var author = await _author.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Author with id:{id} was not found!");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully Got Author with id:{id}!");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalError(($"{ex.Message} - {ex.InnerException}"));
            }
        }
        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param name="authorCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorCreate)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogInfo($"{ location }: Create Attempted");
                if (authorCreate == null)
                {
                    _logger.LogWarn($"{location}: Empty request was submitted!");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Data was incomplte");
                    return BadRequest();
                }
                var author = _mapper.Map<Author>(authorCreate);
                var isSuccess = await _author.Create(author);
                if (!isSuccess)
                {
                    return InternalError(($"{location} - Creation failed!"));
                }
                _logger.LogInfo($"{location}: create Successfully!");
                return Created("Create", new { author });
            }
            catch (Exception ex)
            {
                return InternalError(($"{ex.Message} - {ex.InnerException}"));
            }
        }

        /// <summary>
        /// Update an author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorUpdate"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorUpdate)
        {
            try
            {
                _logger.LogWarn($"Author with id:{id} Update attempt!");
                if (id < 1 || authorUpdate == null || id != authorUpdate.Id)
                {
                    return BadRequest();
                }
                var isExists = await _author.IsExists(id);
                if (!isExists)
                {
                    _logger.LogInfo($"The author with id:{id} was not found!");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author data with the id:{id} was incomplete!");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorUpdate);
                var isSuccess = await _author.Update(author);
                if (!isSuccess)
                {
                    return InternalError("Upate operation Failed!");
                }
                _logger.LogWarn($"Author with the id:{id} successfully Updated!");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError(($"{ex.Message} - {ex.InnerException}"));
            }
        }
        /// <summary>
        /// detele an author
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _logger.LogWarn($"{location}: Attempted with id:{id}!");
                if (id < 1)
                {
                    return BadRequest();
                }

                var isExists = await _author.IsExists(id);
                if (!isExists)
                {
                    _logger.LogInfo($"{location}: record with id:{id} was not found!");
                    return NotFound();
                }

                var author = await _author.FindById(id);

                var isSuccess = await _author.Delete(author);
                if (!isSuccess)
                {
                    return InternalError("Delete operation Failed!");
                }
                _logger.LogWarn($"{location}: record with the id:{id} was successfully Deleted!");
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError(($"{ex.Message} - {ex.InnerException}"));
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
