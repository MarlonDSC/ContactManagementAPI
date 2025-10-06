using ContactManagement.API.Extensions;
using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.Contacts.Commands.CreateContact;
using ContactManagement.Application.Features.Contacts.Commands.UpdateContact;
using ContactManagement.Domain.Errors;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateContact(CreateContactDto createContactDto)
        {
            var command = CreateContactCommand.FromDto(createContactDto);
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }
            
            var contactDto = result.Value!;
            return CreatedAtAction(nameof(GetContact), new { id = contactDto.Id }, contactDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult GetContact(Guid id)
        {
            var result = Result<ContactDto>.NotFound(DomainErrors.Contact.NotFound);
            return result.ToActionResult(this);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateContact(Guid id, UpdateContactDto updateContactDto)
        {
            var command = UpdateContactCommand.FromDto(id, updateContactDto);
            var result = await _mediator.Send(command);
            
            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }
            
            return Ok(result.Value);
        }
    }
}
