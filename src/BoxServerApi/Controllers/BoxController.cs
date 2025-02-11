/*
 * BoxServer API
 *
 * API and objects for BoxServer
 *
 * OpenAPI spec version: 1.0.0
 *
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using BoxServer.Hubs;
using BoxServer.Models;
using BoxServer.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BoxServer.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    public class BoxController : ControllerBase
    {
        private readonly IBoxRepository _boxRepository;
        private readonly IHubContext<MessageHub> _messageHub;
        private readonly ILogger<BoxController> _logger;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="boxRepository"></param>
        /// <param name="messageHub"></param>
        public BoxController(ILogger<BoxController> logger, IBoxRepository boxRepository, IHubContext<MessageHub> messageHub)
        {
            _logger = logger;
            _boxRepository = boxRepository;
            _messageHub = messageHub;
        }

        /// <summary>
        /// Delete an existing box
        /// </summary>
        /// <param name="id">Id</param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Box not found</response>
        /// <response code="405">Validation exception</response>
        [HttpDelete]
        [Route("/api/v1/box/{id:int}")]
        [SwaggerOperation("DeleteBox")]
        public async Task<IActionResult> DeleteBox([FromRoute][Required] int id)
        {
            await _messageHub.Clients.All.SendAsync(MessageHub.MessageName, new TypedMessage() { Text = $"Deleting box with id {id}", Type = TypedMessage.Warning});
            var result = await _boxRepository.DeleteBox(id).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Get a list of boxes
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("/api/v1/box")]
        [SwaggerOperation("GetBoxes")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Box>), description: "successful operation")]
        public async Task<ActionResult<List<Box>>> GetBox()
        {
            var allBoxes = (await _boxRepository.GetBoxes().ConfigureAwait(false))?.ToList();
            if (!allBoxes?.Any() ?? false)
            {
                return NotFound();
            }
            await _messageHub.Clients.All.SendAsync(MessageHub.MessageName, new TypedMessage { Text = "Getting all boxes", Type = TypedMessage.Information});
            return Ok(allBoxes);
        }

        /// <summary>
        /// Get a box by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="405">Validation exception</response>
        [HttpGet]
        [Route("/api/v1/box/{id:int}")]
        [SwaggerOperation("GetBox")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Box>), description: "successful operation")]
        public async Task<ActionResult<Box>> GetBox([FromRoute][Required] int id)
        {
            var ret = await _boxRepository.GetBox(id).ConfigureAwait(false);
            if (ret == null)
            {
                return NotFound();
            }
            await _messageHub.Clients.All.SendAsync(MessageHub.MessageName, new Message { Text = $"Getting box {id}" });
            return Ok(ret);
        }

        /// <summary>
        /// Create a new box
        /// </summary>
        /// <param name="box">Box object that needs to be added</param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="405">Validation exception</response>
        [HttpPost]
        [Route("/api/v1/box")]
        [SwaggerOperation("NewBox")]
        public async Task<ActionResult<Box?>> NewBox([FromBody] Box box)
        {
            if (box.BoxId is not null && box.BoxId != 0)
            {
                return BadRequest("Must not have id on new");
            }
            await _messageHub.Clients.All.SendAsync(MessageHub.MessageName, new Message { Text = $"Adding box {box.Name}" });

            return await _boxRepository.AddBox(box).ConfigureAwait(false);
        }

        /// <summary>
        /// Update an existing box to create a new version
        /// </summary>
        /// <param name="box">Box object that needs to be updated</param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Box not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/api/v1/box")]
        [SwaggerOperation("UpdateBox")]
        public async Task<ActionResult<Box?>> UpdateBox([FromBody] Box box)
        {
            if (box.BoxId == 0)
            {
                return BadRequest("Must have id on update");
            }
            await _messageHub.Clients.All.SendAsync(MessageHub.MessageName, new Message { Text = $"Updating box {box.BoxId}" });
            return await _boxRepository.UpdateBox(box).ConfigureAwait(false);
        }
    }
}