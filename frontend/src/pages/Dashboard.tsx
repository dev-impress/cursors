import React, { useEffect, useMemo, useState } from 'react'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
} from 'chart.js'
import { Line, Bar, Pie } from 'react-chartjs-2'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, Title, Tooltip, Legend, ArcElement)

type Props = { receiptsUrl: string }

export function Dashboard({ receiptsUrl }: Props) {
  const [daily, setDaily] = useState<{ day: string; total: number }[]>([])
  const [monthlyAvg, setMonthlyAvg] = useState<{ year: number; month: number; avgPerDay: number }[]>([])
  const [share, setShare] = useState<{ category: string; total: number }[]>([])

  useEffect(() => {
    const now = new Date()
    const from = new Date(now.getFullYear(), now.getMonth(), 1)
    const to = new Date(now.getFullYear(), now.getMonth() + 1, 0)

    const qsDaily = `from=${from.toISOString().slice(0,10)}&to=${to.toISOString().slice(0,10)}`
    const qsMonthly = `yearFrom=${now.getFullYear()-1}&monthFrom=${now.getMonth()+1}&yearTo=${now.getFullYear()}&monthTo=${now.getMonth()+1}`

    Promise.all([
      fetch(`${receiptsUrl}/api/analytics/daily?${qsDaily}`),
      fetch(`${receiptsUrl}/api/analytics/monthly-avg?${qsMonthly}`),
      fetch(`${receiptsUrl}/api/analytics/current-month-share`),
    ]).then(async ([d, m, s]) => {
      setDaily(await d.json())
      setMonthlyAvg(await m.json())
      setShare(await s.json())
    })
  }, [receiptsUrl])

  const dailyData = useMemo(() => {
    const labels = daily.map(x => x.day)
    return {
      labels,
      datasets: [{ label: 'Daily Spend', data: daily.map(x => x.total), borderColor: '#2563eb', backgroundColor: 'rgba(37,99,235,0.2)' }]
    }
  }, [daily])

  const monthlyAvgData = useMemo(() => {
    const labels = monthlyAvg.map(x => `${x.year}-${String(x.month).padStart(2,'0')}`)
    return {
      labels,
      datasets: [{ label: 'Avg Daily Spend', data: monthlyAvg.map(x => x.avgPerDay), backgroundColor: 'rgba(16,185,129,0.6)' }]
    }
  }, [monthlyAvg])

  const shareData = useMemo(() => {
    const total = share.reduce((acc, s) => acc + s.total, 0)
    const labels = share.map(s => s.category)
    const data = share.map(s => s.total)
    return {
      labels,
      datasets: [{
        label: `Share (${total.toFixed(2)})`,
        data,
        backgroundColor: ['#60a5fa','#f472b6','#34d399','#fbbf24','#a78bfa','#fb7185','#22d3ee','#fde047','#4ade80']
      }]
    }
  }, [share])

  return (
    <div>
      <h2>Dashboard</h2>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr', gap: 20 }}>
        <div style={{ padding: 12, border: '1px solid #e5e7eb', borderRadius: 8 }}>
          <h3 style={{ margin: '0 0 8px' }}>Суточные траты (текущий месяц)</h3>
          <Line data={dailyData} options={{ responsive: true, plugins: { legend: { position: 'top' as const } } }} />
        </div>
        <div style={{ padding: 12, border: '1px solid #e5e7eb', borderRadius: 8 }}>
          <h3 style={{ margin: '0 0 8px' }}>Среднесуточные траты по месяцам</h3>
          <Bar data={monthlyAvgData} options={{ responsive: true, plugins: { legend: { position: 'top' as const } } }} />
        </div>
        <div style={{ padding: 12, border: '1px solid #e5e7eb', borderRadius: 8 }}>
          <h3 style={{ margin: '0 0 8px' }}>Траты по категориям (текущий месяц)</h3>
          <Pie data={shareData} />
        </div>
      </div>
    </div>
  )
}
