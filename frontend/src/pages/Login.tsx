import React, { useState } from 'react'
import { fetchToken, KeycloakConfig } from '../auth'

type Props = { keycloak: KeycloakConfig, token: string | null, onToken: (t: string | null) => void }

export function Login({ keycloak, token, onToken }: Props) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)

  async function login() {
    try {
      setLoading(true)
      const t = await fetchToken(keycloak, username, password)
      onToken(t)
    } catch (e) {
      alert('Login failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ display: 'flex', gap: 8, alignItems: 'center', marginBottom: 12 }}>
      {token ? (
        <>
          <span>Authenticated</span>
          <button onClick={() => onToken(null)}>Logout</button>
        </>
      ) : (
        <>
          <input placeholder="Username" value={username} onChange={e=>setUsername(e.target.value)} />
          <input type="password" placeholder="Password" value={password} onChange={e=>setPassword(e.target.value)} />
          <button onClick={login} disabled={loading || !username || !password}>Login</button>
        </>
      )}
    </div>
  )
}
