import 'react-bootstrap';
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css';
import ValuablesGetter from '../components/ValuablesGetter';
import { useLocation, useNavigate } from 'react-router';
import { Button } from 'react-bootstrap';

export default function PersonalItemsPage() {
    const location = useLocation();
    const navigate = useNavigate()
    const uploadedData = location.state?.uploadedData; // Personal items

    return (
        <div className="min-vh-100 d-flex flex-column">
            <header className="bg-primary text-white p-4 text-center fw-bold fs-4">
                Uh-Surance
            </header>
            
            <main className="flex-grow-1 d-flex align-items-start justify-content-center flex-column" style={{ paddingTop: "5vh" }}>
                <div className="container text-center">
                    <div className="row justify-content-center">
                        <section className="col-md-8 bg-light p-4 rounded shadow">
                            <h2 className="fw-semibold">We've Identified these personal items covered in your policy</h2>
                            <p className="text-muted">Continue to upload photos of each</p>
                        </section>
                    </div>
                    <div className="row justify-content-center">
                        <section className="col-md-8 bg-light p-4 rounded shadow">
                            <h2 className="fw-semibold">PLACEHOLDER FOR ITEMS: {uploadedData}</h2>
                        </section>
                    </div>
                </div>
                <div className="container text-center mt-4">
                    <div className="row justify-content-center">
                        <div className="col-md-6 bg-secondary text-white p-3 rounded shadow">
                            <ValuablesGetter/>
                        </div>
                    </div>
                </div>
            </main>
            
            <footer className="bg-primary text-white p-4 text-center"/>
        </div>
    );
}