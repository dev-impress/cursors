import React, { useEffect, useState } from 'react'

type Category = { id: string; name: string }

type Store = { id: string; name: string; categoryId?: string | null; categoryName?: string | null }

type Props = { baseUrl: string, token: string | null }

export function Stores({ baseUrl, token }: Props) {
  const [items, setItems] = useState<Store[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [name, setName] = useState('')
  const [categoryId, setCategoryId] = useState<string>('')

  async function load() {
    const headers = token ? { Authorization: `Bearer ${token}` } : {}
    const [storesRes, catsRes] = await Promise.all([
      fetch(`${baseUrl}/api/stores`, { headers }),
      fetch(`${baseUrl}/api/categories`, { headers }),
    ])
    setItems(await storesRes.json())
    setCategories(await catsRes.json())
  }

  useEffect(() => { load() }, [])

  async function create() {
    const body = { name, categoryId: categoryId || null }
    await fetch(`${baseUrl}/api/stores`, { method: 'POST', headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) }, body: JSON.stringify(body) })
    setName(''); setCategoryId(''); await load()
  }

  async function update(id: string, newName: string, newCategoryId: string | null) {
    await fetch(`${baseUrl}/api/stores/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) }, body: JSON.stringify({ name: newName, categoryId: newCategoryId }) })
    await load()
  }

  async function remove(id: string) {
    await fetch(`${baseUrl}/api/stores/${id}`, { method: 'DELETE', headers: token ? { Authorization: `Bearer ${token}` } : {} })
    await load()
  }

  return (
    <div>
      <h2>Stores</h2>
      <div style={{ display: 'flex', gap: 8, marginBottom: 12 }}>
        <input placeholder="Name" value={name} onChange={e=>setName(e.target.value)} />
        <select value={categoryId} onChange={e=>setCategoryId(e.target.value)}>
          <option value="">No category</option>
          {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
        <button onClick={create} disabled={!name.trim()}>Create</button>
      </div>
      <table width="100%" cellPadding={6} style={{ borderCollapse: 'collapse' }}>
        <thead><tr><th align="left">Name</th><th align="left">Category</th><th>Actions</th></tr></thead>
        <tbody>
          {items.map(x => (
            <tr key={x.id}>
              <td>{x.name}</td>
              <td>{x.categoryName ?? ''}</td>
              <td>
                <button onClick={() => {
                  const newName = prompt('New name', x.name) ?? x.name
                  const newCatId = prompt('New categoryId (empty for none)', x.categoryId ?? '') ?? (x.categoryId ?? '')
                  update(x.id, newName, newCatId || null)
                }}>Edit</button>
                <button onClick={() => remove(x.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
