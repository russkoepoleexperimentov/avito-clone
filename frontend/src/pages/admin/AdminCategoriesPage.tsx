import { useState } from 'react'
import { useCategories, useCategoryMutations } from '../../features/categories/hooks'
import { flattenCategories } from '../../features/categories/utils'
import type { CategoryInput } from '../../features/categories/api'
import { getApiErrorMessage } from '../../lib/errors'

const empty: CategoryInput = { name: '', slug: '', parentId: null, sortOrder: 0 }
const fieldClass = 'rounded-md border border-gray-300 px-3 py-2 text-sm outline-none focus:border-brand-500'

export function AdminCategoriesPage() {
  const { data: categories } = useCategories()
  const { create, update, remove } = useCategoryMutations()
  const [editingId, setEditingId] = useState<string | null>(null)
  const [form, setForm] = useState<CategoryInput>(empty)

  const options = categories ? flattenCategories(categories) : []
  const busy = create.isPending || update.isPending
  const error = create.error ?? update.error ?? remove.error

  const submit = () => {
    const data: CategoryInput = { ...form, name: form.name.trim(), slug: form.slug.trim() }
    const done = () => {
      setForm(empty)
      setEditingId(null)
    }
    if (editingId) update.mutate({ id: editingId, data }, { onSuccess: done })
    else create.mutate(data, { onSuccess: done })
  }

  const startEdit = (id: string, name: string, slug: string, parentId: string | null, sortOrder: number) => {
    setEditingId(id)
    setForm({ name, slug, parentId, sortOrder })
  }

  return (
    <div className="space-y-6">
      {/* Форма создания/редактирования */}
      <div className="rounded-lg border border-gray-200 bg-white p-4">
        <h2 className="mb-3 font-semibold">
          {editingId ? 'Редактировать категорию' : 'Новая категория'}
        </h2>
        <div className="flex flex-wrap gap-3">
          <input
            className={fieldClass}
            placeholder="Название"
            value={form.name}
            onChange={(e) => setForm({ ...form, name: e.target.value })}
          />
          <input
            className={fieldClass}
            placeholder="slug (латиница)"
            value={form.slug}
            onChange={(e) => setForm({ ...form, slug: e.target.value })}
          />
          <select
            className={fieldClass}
            value={form.parentId ?? ''}
            onChange={(e) => setForm({ ...form, parentId: e.target.value || null })}
          >
            <option value="">— без родителя —</option>
            {options
              .filter((o) => o.id !== editingId)
              .map((o) => (
                <option key={o.id} value={o.id}>
                  {o.label}
                </option>
              ))}
          </select>
          <input
            type="number"
            className={`${fieldClass} w-24`}
            placeholder="Порядок"
            value={form.sortOrder}
            onChange={(e) => setForm({ ...form, sortOrder: Number(e.target.value) })}
          />
          <button
            onClick={submit}
            disabled={busy || !form.name || !form.slug}
            className="rounded-md bg-brand-600 px-4 py-2 text-sm font-medium text-white hover:bg-brand-700 disabled:opacity-50"
          >
            {editingId ? 'Сохранить' : 'Добавить'}
          </button>
          {editingId && (
            <button
              onClick={() => {
                setEditingId(null)
                setForm(empty)
              }}
              className="rounded-md px-3 py-2 text-sm text-gray-500 hover:bg-gray-100"
            >
              Отмена
            </button>
          )}
        </div>
        {error && <p className="mt-2 text-sm text-red-600">{getApiErrorMessage(error)}</p>}
      </div>

      {/* Дерево категорий */}
      <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
        <ul className="divide-y divide-gray-100">
          {categories?.map((root) => (
            <CategoryRows
              key={root.id}
              node={root}
              depth={0}
              onEdit={startEdit}
              onDelete={(id, name) => {
                if (confirm(`Удалить категорию «${name}»?`)) remove.mutate(id)
              }}
            />
          ))}
        </ul>
      </div>
    </div>
  )
}

interface RowsProps {
  node: import('../../features/categories/types').Category
  depth: number
  onEdit: (id: string, name: string, slug: string, parentId: string | null, sortOrder: number) => void
  onDelete: (id: string, name: string) => void
}

function CategoryRows({ node, depth, onEdit, onDelete }: RowsProps) {
  return (
    <>
      <li className="flex items-center gap-2 p-3">
        <span style={{ paddingLeft: depth * 16 }} className="flex-1">
          <span className="font-medium">{node.name}</span>
          <span className="ml-2 text-xs text-gray-400">/{node.slug}</span>
        </span>
        <button
          onClick={() => onEdit(node.id, node.name, node.slug, node.parentId, node.sortOrder)}
          className="rounded-md px-2 py-1 text-xs text-brand-600 hover:bg-brand-50"
        >
          Изменить
        </button>
        <button
          onClick={() => onDelete(node.id, node.name)}
          className="rounded-md px-2 py-1 text-xs text-red-600 hover:bg-red-50"
        >
          Удалить
        </button>
      </li>
      {node.children.map((child) => (
        <CategoryRows key={child.id} node={child} depth={depth + 1} onEdit={onEdit} onDelete={onDelete} />
      ))}
    </>
  )
}
