using Application.Common.Pagination;
using Application.DTOs;
using Application.Filters;
using Application.Interfaces.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/documents")]
[ApiVersion("1.0")]
public class MetaDocumentController : Controller
{
    private readonly IDocumentMetadataService _documentMetadataService;

    public MetaDocumentController(
        IDocumentMetadataService documentMetadataService)
    {
        _documentMetadataService = documentMetadataService;
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(PaginatedList<DocumentMetadataDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInvites(
    [FromQuery] PaginationParams pagination,
    [FromQuery] DocumentMetadataFilter filter,
    CancellationToken cancellationToken = default)
    {
        var result = await _documentMetadataService.GetPaginatedAsync(
             filter,
             pagination,
             cancellationToken);

        return Ok(result);
    }
}
