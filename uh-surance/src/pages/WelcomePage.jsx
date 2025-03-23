import 'react-bootstrap'
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css'

export default function WelcomePage() {

    return (
        <div className="min-vh-100 d-flex flex-column">

      <header className="bg-primary text-white p-4 text-center fw-bold">
        Uh-Surance
      </header>

      <main className="flex-grow-1 p-4 container">
        <div className="row g-4">
          <section className="row-md-6 bg-light p-4 rounded shadow">
            <h2 className="fw-semibold">Section 1</h2>
            <p className="text-muted">This is some content for section 1.</p>
          </section>
          <section className="row-md-6 bg-light p-4 rounded shadow">
            <h2 className="fw-semibold">Section 2</h2>
            <p className="text-muted">This is some content for section 2.</p>
          </section>
        </div>
      </main>

      <footer className="bg-primary text-white p-4 text-center"/>
    </div>
    )
}