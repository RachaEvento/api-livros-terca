using FluentValidation;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Abstractions.Library;
using MeuAcervo.Application.Abstractions.Reviews;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Reviews;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Services.Reviews;

public sealed class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserLibraryRepository _userLibraryRepository;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IValidator<UpsertReviewRequest> _upsertReviewRequestValidator;

    public ReviewService(
        IReviewRepository reviewRepository,
        IUserLibraryRepository userLibraryRepository,
        IApplicationDbContext applicationDbContext,
        IValidator<UpsertReviewRequest> upsertReviewRequestValidator)
    {
        _reviewRepository = reviewRepository;
        _userLibraryRepository = userLibraryRepository;
        _applicationDbContext = applicationDbContext;
        _upsertReviewRequestValidator = upsertReviewRequestValidator;
    }

    public async Task<ReviewResponse> GetByLibraryItemAsync(Guid tenantId, Guid userId, Guid libraryItemId, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByLibraryItemAsync(tenantId, userId, libraryItemId, tracking: false, cancellationToken)
                     ?? throw new NotFoundException("Review was not found for the informed library item.");

        return MapResponse(review);
    }

    public async Task<ReviewResponse> CreateAsync(Guid tenantId, Guid userId, Guid libraryItemId, UpsertReviewRequest request, CancellationToken cancellationToken = default)
    {
        await _upsertReviewRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var libraryItem = await _userLibraryRepository.GetTrackedItemAsync(tenantId, userId, libraryItemId, cancellationToken)
                          ?? throw new NotFoundException("Library item was not found for the authenticated user.");

        var existingReview = await _reviewRepository.GetByLibraryItemAsync(tenantId, userId, libraryItemId, tracking: true, cancellationToken);
        if (existingReview is not null)
        {
            throw new ConflictException("The informed library item already has an active review.");
        }

        var review = new Review
        {
            TenantId = tenantId,
            UserId = userId,
            UserLibraryItemId = libraryItemId,
            UserLibraryItem = libraryItem,
            Rating = request.Rating,
            Title = TrimOrNull(request.Title),
            Content = request.Content.Trim(),
            Visibility = request.Visibility,
            ContainsSpoilers = request.ContainsSpoilers,
            PublishedAtUtc = request.Visibility == ReviewVisibility.Public ? DateTime.UtcNow : null
        };

        _reviewRepository.Add(review);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return MapResponse(review);
    }

    public async Task<ReviewResponse> UpdateAsync(Guid tenantId, Guid userId, Guid libraryItemId, UpsertReviewRequest request, CancellationToken cancellationToken = default)
    {
        await _upsertReviewRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var review = await _reviewRepository.GetByLibraryItemAsync(tenantId, userId, libraryItemId, tracking: true, cancellationToken)
                     ?? throw new NotFoundException("Review was not found for the informed library item.");

        review.Rating = request.Rating;
        review.Title = TrimOrNull(request.Title);
        review.Content = request.Content.Trim();
        review.Visibility = request.Visibility;
        review.ContainsSpoilers = request.ContainsSpoilers;
        review.PublishedAtUtc = request.Visibility == ReviewVisibility.Public
            ? review.PublishedAtUtc ?? DateTime.UtcNow
            : null;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        return MapResponse(review);
    }

    public async Task DeleteAsync(Guid tenantId, Guid userId, Guid libraryItemId, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByLibraryItemAsync(tenantId, userId, libraryItemId, tracking: true, cancellationToken)
                     ?? throw new NotFoundException("Review was not found for the informed library item.");

        _reviewRepository.Remove(review);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    private static ReviewResponse MapResponse(Review review)
    {
        return new ReviewResponse(
            review.Id,
            review.Rating,
            review.Title,
            review.Content,
            review.Visibility,
            review.ContainsSpoilers,
            review.PublishedAtUtc,
            review.CreatedAtUtc,
            review.UpdatedAtUtc);
    }

    private static string? TrimOrNull(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
