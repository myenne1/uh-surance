import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './styles/App.css'
import WelcomePage from './pages/WelcomePage'
import PhotosPage from './pages/PhotosPage'
import SummarizationPage from './pages/SummarizationPage'
import PersonalItemsPage from './pages/PersonalItemsPage'
import DashboardPage from './pages/DashboardPage'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";


function App() {
  const [count, setCount] = useState(0)

  return (
    <Router>
      <Routes>
        <Route path="/" element={<WelcomePage/>}/>
        <Route path="/summarization-page" element={<SummarizationPage/>}/>
        <Route path="/photos-page" element={<PhotosPage/>}/>
        <Route path="/personal-items-page" element={<PersonalItemsPage/>}/>
        <Route path='/dashboard-page' element={<DashboardPage/>}/>
      </Routes>
    </Router>
  )
}

export default App
