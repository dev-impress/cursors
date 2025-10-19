import React, { useEffect, useState } from 'react'

type Category = { id: string; name: string; description?: string | null }

type Props = { baseUrl: string, token: string | null }

export function Categories({ baseUrl, token }: Props) {
  const [items, setItems] = useState<Category[]>([])
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')

  async function load() {
    const res = await fetch(`${baseUrl}/api/categories`, { headers: token ? { Authorization: `Bearer ${token}` } : {} })
    setItems(await res.json())
  }

  useEffect(() => { load() }, [])

  async function create() {
    await fetch(`${baseUrl}/api/categories`, {
      method: 'POST', headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) },
      body: JSON.stringify({ name, description })
    })
    setName(''); setDescription(''); await load()
  }

  async function update(id: string, newName: string, newDescription: string) {
    await fetch(`${baseUrl}/api/categories/${id}`, {
      method: 'PUT', headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) },
      body: JSON.stringify({ name: newName, description: newDescription })
    })
    await load()
  }

  async function remove(id: string) {
    await fetch(`${baseUrl}/api/categories/${id}`, { method: 'DELETE', headers: token ? { Authorization: `Bearer ${token}` } : {} })
    await load()
  }

  return (
    <div>
      <h2>Categories</h2>
      <div style={{ display: 'flex', gap: 8, marginBottom: 12 }}>
        <input placeholder="Name" value={name} onChange={e=>setName(e.target.value)} />
        <input placeholder="Description" value={description} onChange={e=>setDescription(e.target.value)} />
        <button onClick={create} disabled={!name.trim()}>Create</button>
      </div>
      <table width="100%" cellPadding={6} style={{ borderCollapse: 'collapse' }}>
        <thead><tr><th align="left">Name</th><th align="left">Description</th><th>Actions</th></tr></thead>
        <tbody>
          {items.map(x => (
            <tr key={x.id}>
              <td>{x.name}</td>
              <td>{x.description ?? ''}</td>
              <td>
                <button onClick={() => {
                  const newName = prompt('New name', x.name) ?? x.name
                  const newDesc = prompt('New description', x.description ?? '') ?? x.description ?? ''
                  update(x.id, newName, newDesc)
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
