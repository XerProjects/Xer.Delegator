using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xer.Delegator;

namespace Domain.Repositories
{
    public class PublishingProductRepository : IProductRepository
    {
        private readonly IMessageDelegator _messageDelegator;
        private readonly IProductRepository _inner;

        public PublishingProductRepository(IProductRepository inner, IMessageDelegator messageDelegator)
        {
            _inner = inner;
            _messageDelegator = messageDelegator ?? throw new System.ArgumentNullException(nameof(messageDelegator));
        }
        
        public Task<Product> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _inner.GetProductByIdAsync(productId, cancellationToken);
        }

        public async Task SaveAsync(Product product, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get copy.
            IReadOnlyCollection<IDomainEvent> uncommittedDomainEvents = product.GetUncommittedDomainEvents();

            // Do actual save.
            await _inner.SaveAsync(product, cancellationToken);
            // Send each domain events to handlers.
            await _messageDelegator.SendAllAsync(uncommittedDomainEvents);
        }
    }
}