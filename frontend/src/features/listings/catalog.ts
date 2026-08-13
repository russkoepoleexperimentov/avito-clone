import { api } from '../../lib/api'
import type { ListingListItem } from './types'

export type SortOption = 'newest' | 'price_asc' | 'price_desc'

export interface ListingQuery {
  search?: string
  categorySlug?: string
  minPrice?: number
  maxPrice?: number
  city?: string
  sort?: SortOption
  page?: number
  pageSize?: number
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export async function getListings(query: ListingQuery): Promise<PagedResult<ListingListItem>> {
  const res = await api.get<PagedResult<ListingListItem>>('/listings', { params: query })
  return res.data
}
