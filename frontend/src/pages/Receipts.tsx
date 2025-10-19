import React, { useEffect, useState } from 'react'

type Receipt = { id: string; issuedAt: string; storeName: string; currency: string; total: number; payload: string }

type Props = { baseUrl: string }

export function Receipts({ baseUrl }: Props) {
  const [items, setItems] = useState<Receipt[]>([])

  const [issuedAt, setIssuedAt] = useState<string>(() => new Date().toISOString().slice(0,16))
  const [storeName, setStoreName] = useState('')
  const [currency, setCurrency] = useState('USD')
  const [total, setTotal] = useState<string>('0')
  const [payload, setPayload] = useState('')

  async function load() {
    const res = await fetch(`${baseUrl}/api/receipts`)
    setItems(await res.json())
  }

  useEffect(() => { load() }, [])

  async function create() {
    await fetch(`${baseUrl}/api/receipts`, {
      method: 'POST', headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ issuedAt: new Date(issuedAt), storeName, currency, total: parseFloat(total), payload })
    })
    setStoreName(''); setTotal('0'); setPayload(''); await load()
  }

  return (
    <div>
      <h2>Receipts</h2>
      <div style={{ display: 'flex', gap: 8, marginBottom: 12, flexWrap: 'wrap' }}>
        <input type="datetime-local" value={issuedAt} onChange={e=>setIssuedAt(e.target.value)} />
        <input placeholder="Store name" value={storeName} onChange={e=>setStoreName(e.target.value)} />
        <input placeholder="Currency" value={currency} onChange={e=>setCurrency(e.target.value)} />
        <input type="number" min="0" step="0.01" placeholder="Total" value={total} onChange={e=>setTotal(e.target.value)} />
        <input placeholder="Payload (JSON or text)" value={payload} onChange={e=>setPayload(e.target.value)} style={{ flex: 1 }} />
        <button onClick={create} disabled={!storeName.trim() || !payload.trim()}>Save</button>
      </div>
      <table width="100%" cellPadding={6} style={{ borderCollapse: 'collapse' }}>
        <thead><tr><th align="left">Issued At</th><th align="left">Store</th><th align="left">Currency</th><th align="right">Total</th><th align="left">Payload</th></tr></thead>
        <tbody>
          {items.map(x => (
            <tr key={x.id}>
              <td>{new Date(x.issuedAt).toLocaleString()}</td>
              <td>{x.storeName}</td>
              <td>{x.currency}</td>
              <td align="right">{x.total.toFixed(2)}</td>
              <td><pre style={{ whiteSpace: 'pre-wrap', margin: 0 }}>{x.payload}</pre></td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
