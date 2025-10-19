import React, { useEffect, useState } from 'react'

type Category = { id: string; name: string }

type Store = { id: string; name: string; categoryId?: string | null; categoryName?: string | null }

type Product = {
  id: string
  name: string
  price: number
  storeId: string
  storeName: string
  categoryId?: string | null
  categoryName?: string | null
  effectiveCategoryId?: string | null
  effectiveCategoryName?: string | null
}

type Props = { baseUrl: string }

export function Products({ baseUrl }: Props) {
  const [items, setItems] = useState<Product[]>([])
  const [stores, setStores] = useState<Store[]>([])
  const [categories, setCategories] = useState<Category[]>([])

  const [name, setName] = useState('')
  const [price, setPrice] = useState<string>('0')
  const [storeId, setStoreId] = useState('')
  const [categoryId, setCategoryId] = useState('')

  async function load() {
    const [prodsRes, storesRes, catsRes] = await Promise.all([
      fetch(`${baseUrl}/api/products`),
      fetch(`${baseUrl}/api/stores`),
      fetch(`${baseUrl}/api/categories`),
    ])
    setItems(await prodsRes.json())
    setStores(await storesRes.json())
    setCategories(await catsRes.json())
  }

  useEffect(() => { load() }, [])

  async function create() {
    const body = { name, price: parseFloat(price), storeId, categoryId: categoryId || null }
    await fetch(`${baseUrl}/api/products`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) })
    setName(''); setPrice('0'); setStoreId(''); setCategoryId(''); await load()
  }

  async function update(id: string, newName: string, newPrice: number, newCategoryId: string | null) {
    await fetch(`${baseUrl}/api/products/${id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ name: newName, price: newPrice, categoryId: newCategoryId }) })
    await load()
  }

  async function remove(id: string) {
    await fetch(`${baseUrl}/api/products/${id}`, { method: 'DELETE' })
    await load()
  }

  return (
    <div>
      <h2>Products</h2>
      <div style={{ display: 'flex', gap: 8, marginBottom: 12, flexWrap: 'wrap' }}>
        <input placeholder="Name" value={name} onChange={e=>setName(e.target.value)} />
        <input type="number" min="0" step="0.01" placeholder="Price" value={price} onChange={e=>setPrice(e.target.value)} />
        <select value={storeId} onChange={e=>setStoreId(e.target.value)}>
          <option value="">Select store</option>
          {stores.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
        </select>
        <select value={categoryId} onChange={e=>setCategoryId(e.target.value)}>
          <option value="">No category</option>
          {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
        <button onClick={create} disabled={!name.trim() || !storeId}>Create</button>
      </div>
      <table width="100%" cellPadding={6} style={{ borderCollapse: 'collapse' }}>
        <thead><tr><th align="left">Name</th><th align="left">Store</th><th align="right">Price</th><th align="left">Product Category</th><th align="left">Effective Category</th><th>Actions</th></tr></thead>
        <tbody>
          {items.map(x => (
            <tr key={x.id}>
              <td>{x.name}</td>
              <td>{x.storeName}</td>
              <td align="right">{x.price.toFixed(2)}</td>
              <td>{x.categoryName ?? ''}</td>
              <td>{x.effectiveCategoryName ?? ''}</td>
              <td>
                <button onClick={() => {
                  const newName = prompt('New name', x.name) ?? x.name
                  const newPriceStr = prompt('New price', String(x.price)) ?? String(x.price)
                  const newPrice = parseFloat(newPriceStr)
                  const newCatId = prompt('New categoryId (empty for none)', x.categoryId ?? '') ?? (x.categoryId ?? '')
                  update(x.id, newName, newPrice, newCatId || null)
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
