export type KeycloakConfig = { url: string; realm: string; clientId: string }

export async function fetchToken(config: KeycloakConfig, username: string, password: string) {
  const url = `${config.url}/realms/${config.realm}/protocol/openid-connect/token`
  const form = new URLSearchParams()
  form.set('grant_type', 'password')
  form.set('client_id', config.clientId)
  form.set('username', username)
  form.set('password', password)
  const res = await fetch(url, { method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded' }, body: form.toString() })
  if (!res.ok) throw new Error('Auth failed')
  const json = await res.json()
  return json.access_token as string
}
