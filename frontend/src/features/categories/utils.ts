import type { Category } from './types'

export interface CategoryOption {
  id: string
  label: string
  isLeaf: boolean
}

/**
 * Разворачивает дерево категорий в плоский список для <select>.
 * Подкатегории показываются с отступом; выбирать имеет смысл листья.
 */
export function flattenCategories(categories: Category[], depth = 0): CategoryOption[] {
  return categories.flatMap((c) => [
    { id: c.id, label: `${'  '.repeat(depth)}${c.name}`, isLeaf: c.children.length === 0 },
    ...flattenCategories(c.children, depth + 1),
  ])
}

/** Ищет категорию по slug в дереве (в глубину). */
export function findCategoryBySlug(categories: Category[], slug: string): Category | undefined {
  for (const c of categories) {
    if (c.slug === slug) return c
    const found = findCategoryBySlug(c.children, slug)
    if (found) return found
  }
  return undefined
}
