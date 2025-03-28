import 'react-bootstrap';
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css';
import PhotosGetter from '../components/PhotosGetter';

export default function PhotosPage() {
    return (
        <div className="min-vh-100 d-flex flex-column">
            <header className="bg-primary text-white p-4 text-center fw-bold fs-4">
                Uh-Surance
            </header>
            
            <main className="flex-grow-1 d-flex align-items-start justify-content-center flex-column" style={{ paddingTop: "5vh" }}>
                <div className="container text-center">
                    <div className="row justify-content-center">
                        <section className="col-md-8 bg-light p-4 rounded shadow">
                            <h2 className="fw-semibold">Great! Let us guide you further.</h2>
                            <p className="text-muted">Take 5 photos of your home for safekeeping!</p>
                        </section>
                    </div>
                </div>
                <div className="container text-center mt-4">
                    <div className="row justify-content-center">
                        <div className="col-md-6 bg-secondary text-white p-3 rounded shadow">
                            <PhotosGetter/>
                        </div>
                    </div>
                </div>
            </main>
            
            <footer className="bg-primary text-white p-4 text-center"/>
        </div>
    );
}