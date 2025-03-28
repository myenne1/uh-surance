import 'react-bootstrap';
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css';
import PolicyGetter from '../components/PolicyGetter';

export default function WelcomePage() {
    return (
        <div className="min-vh-100 d-flex flex-column">
            <header className="bg-primary text-white p-4 text-center fw-bold fs-4">
                Uh-Surance
            </header>
            
            <main className="flex-grow-1 d-flex align-items-start justify-content-center flex-column" style={{ paddingTop: "5vh" }}>
                <div className="container text-center">
                    <div className="row justify-content-center">
                        <section className="col-md-8 bg-light p-4 rounded shadow">
                            <h2 className="fw-semibold">Life happens fast, and when the unexpected occurs, being prepared makes all the difference.</h2>
                            <p className="text-muted">Uh-Surance helps you take control of your insurance coverage by documenting what matters most
                                before you need to make a claim. By uploading your insurance policy now, we'll guide you
                                through capturing the photos you need for comprehensive protection. No more scrambling for
                                documentation during stressful situations—let's build your digital insurance record together,
                                starting with uploading your policy.</p>
                        </section>
                    </div>
                </div>
                <div className="container text-center mt-4">
                    <div className="row justify-content-center">
                        <div className="col-md-6 bg-secondary text-white p-3 rounded shadow">
                            <PolicyGetter/>
                        </div>
                    </div>
                </div>
            </main>
            
            <footer className="bg-primary text-white p-4 text-center"/>
        </div>
    );
}