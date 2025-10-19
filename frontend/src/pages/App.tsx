import React, { useMemo, useState } from 'react'
import { Categories } from './Categories'
import { Stores } from './Stores'
import { Products } from './Products'
import { Receipts } from './Receipts'
import { Dashboard } from './Dashboard'
import { Login } from './Login'
import { useAuth } from '../store/authStore'

export function App() {
  const [tab, setTab] = useState<'dashboard'|'categories'|'stores'|'products'|'receipts'>('dashboard')
  const apiBase = useMemo(() => ({
    catalog: import.meta.env.VITE_CATALOG_URL ?? 'http://localhost:5101',
    receipts: import.meta.env.VITE_RECEIPTS_URL ?? 'http://localhost:5102',
  }), [])
  const keycloak = useMemo(() => ({
    url: import.meta.env.VITE_KEYCLOAK_URL ?? 'http://localhost:8081',
    realm: import.meta.env.VITE_KEYCLOAK_REALM ?? 'mvp',
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT ?? 'mvp-web'
  }), [])

  const { token, setToken } = useAuth()

  return (
    <div style={{ fontFamily: 'Inter, system-ui, Arial', padding: 20, maxWidth: 1100, margin: '0 auto' }}>
      <h1>MVP E-Com</h1>
      <Login keycloak={keycloak} token={token} onToken={setToken} />
      <nav style={{ display: 'flex', gap: 8, marginBottom: 16, flexWrap: 'wrap' }}>
        <button onClick={() => setTab('dashboard')} disabled={tab==='dashboard'}>Dashboard</button>
        <button onClick={() => setTab('categories')} disabled={tab==='categories'}>Categories</button>
        <button onClick={() => setTab('stores')} disabled={tab==='stores'}>Stores</button>
        <button onClick={() => setTab('products')} disabled={tab==='products'}>Products</button>
        <button onClick={() => setTab('receipts')} disabled={tab==='receipts'}>Receipts</button>
      </nav>
      {tab === 'dashboard' && <Dashboard receiptsUrl={apiBase.receipts} />}
      {tab === 'categories' && <Categories baseUrl={apiBase.catalog} token={token} />}
      {tab === 'stores' && <Stores baseUrl={apiBase.catalog} token={token} />}
      {tab === 'products' && <Products baseUrl={apiBase.catalog} token={token} />}
      {tab === 'receipts' && <Receipts baseUrl={apiBase.receipts} token={token} />}
    </div>
  )
}
