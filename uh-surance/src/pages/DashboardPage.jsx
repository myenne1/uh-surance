import 'react-bootstrap';
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css';
import PhotosGetter from '../components/PhotosGetter';
import { useState } from 'react';

export default function DashboardPage() {
    // Sample data for policies
    const [policies] = useState([
        { id: 1, type: "Home Insurance", coverage: "$250,000", premium: "$100/month", expiry: "Dec 31, 2025" },
        { id: 2, type: "Auto Insurance", coverage: "$25,000", premium: "$50/month", expiry: "Aug 15, 2025" },
        { id: 3, type: "Personal Property", coverage: "$5,000", premium: "$10/month", expiry: "Oct 22, 2025" }
    ]);

    // State for active tab
    const [activeTab, setActiveTab] = useState('policies');


    return (
        <div className="min-vh-100 d-flex flex-column">
            <header className="bg-primary text-white p-4 text-center fw-bold fs-4">
                Uh-Surance
            </header>
            
            <main className="flex-grow-1 container py-4">
                <div className="row mb-4">
                    <div className="col-12">
                        <h2 className="fw-semibold mb-3">Your Insurance Dashboard</h2>
                        <p className="text-muted">Manage your insurance policies and personal items all in one place.</p>
                    </div>
                </div>

                {/* Navigation Tabs */}
                <ul className="nav nav-tabs mb-4">
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'policies' ? 'active' : ''}`}
                            onClick={() => setActiveTab('policies')}
                        >
                            <i className="bi bi-file-earmark-text me-2"></i>My Policies
                        </button>
                    </li>
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'photos' ? 'active' : ''}`}
                            onClick={() => setActiveTab('photos')}
                        >
                            <i className="bi bi-camera me-2"></i>Personal Item Photos
                        </button>
                    </li>
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'claims' ? 'active' : ''}`}
                            onClick={() => setActiveTab('claims')}
                        >
                            <i className="bi bi-clipboard-check me-2"></i>Miscellaneous
                        </button>
                    </li>
                </ul>

                {/* Policies Tab Content */}
                {activeTab === 'policies' && (
                    <div className="row">
                        <div className="col-md-8">
                            <div className="card shadow-sm mb-4">
                                <div className="card-header bg-light d-flex justify-content-between align-items-center">
                                    <h5 className="m-0">Your Insurance Policies</h5>
                                    <button className="btn btn-sm btn-outline-primary">
                                        <i className="bi bi-plus"></i> Add Policy
                                    </button>
                                </div>
                                <div className="card-body p-0">
                                    <div className="table-responsive">
                                        <table className="table table-hover mb-0">
                                            <thead className="table-light">
                                                <tr>
                                                    <th>Policy Type</th>
                                                    <th>Coverage</th>
                                                    <th>Premium</th>
                                                    <th>Expiry Date</th>
                                                    <th>Actions</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {policies.map(policy => (
                                                    <tr key={policy.id}>
                                                        <td>{policy.type}</td>
                                                        <td>{policy.coverage}</td>
                                                        <td>{policy.premium}</td>
                                                        <td>{policy.expiry}</td>
                                                        <td>
                                                            <button className="btn btn-sm btn-link">View</button>
                                                            <button className="btn btn-sm btn-link">Edit</button>
                                                        </td>
                                                    </tr>
                                                ))}
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card shadow-sm mb-4">
                                <div className="card-header bg-light">
                                    <h5 className="m-0">Policy Summary</h5>
                                </div>
                                <div className="card-body">
                                    <div className="d-flex justify-content-between mb-3">
                                        <span>Total Policies:</span>
                                        <span className="fw-bold">{policies.length}</span>
                                    </div>
                                    <div className="d-flex justify-content-between mb-3">
                                        <span>Monthly Premium:</span>
                                        <span className="fw-bold">$160</span>
                                    </div>
                                    <div className="d-flex justify-content-between">
                                        <span>Total Coverage:</span>
                                        <span className="fw-bold">$280,000</span>
                                    </div>
                                </div>
                                <div className="card-footer bg-white">
                                    <button className="btn btn-primary w-100">Download Summary</button>
                                </div>
                            </div>
                        </div>
                    </div>
                )}

                {/* Photos Tab Content */}
                {activeTab === 'photos' && (
                    <div className="row">
                        <div className="col-md-8">
                            <div className="card shadow-sm mb-4">
                                <div className="card-header bg-light d-flex justify-content-between align-items-center">
                                    <h5 className="m-0">Property Photos</h5>
                                    <span className="badge bg-secondary">15 Photos</span>
                                </div>
                                <div className="card-body">
                                    <p className="mb-3">Take photos of your valuable items to document them for insurance purposes.</p>
                                    <div className="bg-light p-3 rounded mb-3">
                                        <input 
                                            type='file' 
                                            multiple 
                                            accept="image/*"
                                        />
                                    </div>
                                    <h6 className="mt-4 mb-3">Recently Added Photos</h6>
                                    <div className="row g-3">
                                        {[1, 2, 3, 4, 5, 6].map((item) => (
                                            <div key={item} className="col-md-4 col-6">
                                                <div className="card h-100">
                                                    <div className="bg-secondary text-white d-flex align-items-center justify-content-center" style={{height: "120px"}}>
                                                        <i className="bi bi-image" style={{fontSize: "2rem"}}></i>
                                                    </div>
                                                    <div className="card-body p-2">
                                                        <small className="text-muted d-block">Item {item}</small>
                                                        <small className="text-muted">Added: Mar 20, 2025</small>
                                                    </div>
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                                <div className="card-footer bg-white">
                                    <button className="btn btn-outline-primary">View All Photos</button>
                                </div>
                            </div>
                        </div>
                    </div>
                )}
            </main>
            
            <footer className="bg-primary text-white p-3 text-center">
            </footer>
        </div>
    );
}