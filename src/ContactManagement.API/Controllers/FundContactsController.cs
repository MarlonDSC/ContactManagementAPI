using ContactManagement.API.Extensions;
using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.FundContacts.Commands.AssignContactToFund;
using ContactManagement.Application.Features.FundContacts.Commands.RemoveContactFromFund;
using ContactManagement.Application.Features.FundContacts.Queries.GetContactsByFund;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundContactsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(FundContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignContactToFund(CreateFundContactDto createFundContactDto)
        {
            var command = AssignContactToFundCommand.FromDto(createFundContactDto);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            var fundContactDto = result.Value!;
            return CreatedAtAction(
                nameof(GetContactsByFund),
                new { fundId = fundContactDto.FundId },
                fundContactDto);
        }

        [HttpDelete("{contactId}/funds/{fundId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveContactFromFund(Guid contactId, Guid fundId)
        {
            var command = new RemoveContactFromFundCommand(contactId, fundId);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            return NoContent();
        }

        [HttpGet("funds/{fundId}/contacts")]
        [ProducesResponseType(typeof(IEnumerable<FundContactListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContactsByFund(Guid fundId)
        {
            var query = new GetContactsByFundQuery(fundId);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            return Ok(result.Value);
        }
    }
}
