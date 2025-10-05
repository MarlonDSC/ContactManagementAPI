using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Infrastructure.Repositories
{
    public class ContactRepository(ApplicationDbContext dbContext, ILogger<ContactRepository> logger) : BaseRepository<Contact>(dbContext, logger), IContactRepository { }
}
