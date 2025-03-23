import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import WelcomePage from './pages/WelcomePage'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
     <WelcomePage/>
    </>
  )
}

export default App
