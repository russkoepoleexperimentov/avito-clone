import { api } from '../../lib/api'
import type { AdminListingsPage, AdminUser } from './types'

export async function getUsers(): Promise<AdminUser[]> {
  const res = await api.get<AdminUser[]>('/admin/users')
  return res.data
}

export async function setUserBlocked(userId: string, blocked: boolean): Promise<void> {
  await api.put(`/admin/users/${userId}/blocked`, { blocked })
}

export async function getAdminListings(page: number): Promise<AdminListingsPage> {
  const res = await api.get<AdminListingsPage>('/admin/listings', { params: { page, pageSize: 20 } })
  return res.data
}
