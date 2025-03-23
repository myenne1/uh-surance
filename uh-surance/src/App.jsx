import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './styles/App.css'
import WelcomePage from './pages/WelcomePage'
import SecondPage from '../webApi/WebApi/bin/SecondPage'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";


function App() {
  const [count, setCount] = useState(0)

  return (
    <Router>
      <Routes>
        <Route path="/" element={<WelcomePage/>}/>
        <Route path="/second-page" element={<SecondPage/>}/>
      </Routes>
    </Router>
  )
}

export default App
