import { Link } from 'react-router-dom'
import type { ListingListItem } from '../types'
import { formatPrice } from '../../../lib/format'
import { FavoriteButton } from '../../favorites/components/FavoriteButton'

export function ListingCard({ listing }: { listing: ListingListItem }) {
  return (
    <Link
      to={`/listings/${listing.id}`}
      className="group relative overflow-hidden rounded-lg border border-gray-200 bg-white transition-shadow hover:shadow-md"
    >
      <div className="absolute right-2 top-2 z-10">
        <FavoriteButton listingId={listing.id} isFavorite={listing.isFavorite} />
      </div>
      <div className="aspect-square overflow-hidden bg-gray-100">
        {listing.primaryImageUrl ? (
          <img
            src={listing.primaryImageUrl}
            alt={listing.title}
            className="h-full w-full object-cover transition-transform group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-sm text-gray-300">
            Нет фото
          </div>
        )}
      </div>
      <div className="p-3">
        <p className="text-lg font-bold text-gray-900">{formatPrice(listing.price)}</p>
        <p className="truncate text-sm text-gray-700" title={listing.title}>
          {listing.title}
        </p>
        <p className="mt-1 text-xs text-gray-400">{listing.city}</p>
      </div>
    </Link>
  )
}
