using ContactManagement.API.Extensions;
using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.Funds.Commands.CreateFund;
using ContactManagement.Application.Features.Funds.Commands.CreateMultipleFunds;
using ContactManagement.Application.Features.Funds.Queries.GetAllFunds;
using ContactManagement.Application.Features.Funds.Queries.GetFund;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(FundDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateFund(CreateFundDto createFundDto)
        {
            var command = CreateFundCommand.FromDto(createFundDto);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            var fundDto = result.Value!;
            return CreatedAtAction(nameof(GetFund), new { id = fundDto.Id }, fundDto);
        }

        [HttpPost("batch")]
        [ProducesResponseType(typeof(List<FundDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMultipleFunds([FromBody] List<string> fundNames)
        {
            var command = CreateMultipleFundsCommand.FromNames(fundNames);
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            var fundDtos = result.Value!;
            return Created(string.Empty, fundDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FundDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFund(Guid id)
        {
            var query = new GetFundQuery(id);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FundDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFunds([FromQuery] bool includeDeleted = false)
        {
            var query = new GetAllFundsQuery(includeDeleted);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return result.ToActionResult(this);
            }

            return Ok(result.Value);
        }
    }
}
