import 'react-bootstrap';
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css';
import PhotosGetter from '../components/PhotosGetter';
import { useLocation, useNavigate } from 'react-router';
import { Button, Card, Badge } from 'react-bootstrap';
import { FaCalendarAlt, FaFileAlt, FaArrowRight } from 'react-icons/fa';

export default function SummarizationPage() {
    const location = useLocation();
    const navigate = useNavigate();
    const uploadedData = location.state?.uploadedData;

    const handleButtonClick = () => {
        navigate("/photos-page");
    }

    // Format the date to be more readable
    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', { 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }

    // Basic function to get file name without UUID
    const getCleanFileName = (fileName) => {
        if (!fileName) return 'Document';
        return fileName.split('_').slice(1).join('_') || fileName;
    }

    return (
        <div className="min-vh-100 d-flex flex-column bg-light">
            <header className="bg-primary text-white p-4 text-center fw-bold fs-4">
                Uh-Surance
            </header>
            
            <main className="flex-grow-1 d-flex align-items-start justify-content-center" style={{ paddingTop: "5vh" }}>
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-md-8">
                            <Card>
                                <Card.Header className="bg-primary text-white py-3">
                                    <h2 className="mb-0 text-center">Policy Summary</h2>
                                </Card.Header>
                                
                                <Card.Body className="p-4">
                                    {uploadedData ? (
                                        <>
                                            <div className="mb-4 p-3 bg-light rounded">
                                                <div>
                                                    <h5 className="mb-1">
                                                        <FaFileAlt className="me-2 text-primary" />
                                                        {getCleanFileName(uploadedData.fileName)}
                                                    </h5>
                                                    <p className="text-muted mb-0">
                                                        <FaCalendarAlt className="me-2" />
                                                        <small>{formatDate(uploadedData.uploadDate)}</small>
                                                    </p>
                                                </div>
                                                <Badge bg="success" className="px-3 py-2">Analyzed</Badge>
                                            </div>
                                            
                                            <h4 className="mb-3">Key Findings</h4>
                                            <div className="p-3 bg-white rounded">
                                                {uploadedData.summary}
                                            </div>
                                            
                                            <div className="d-flex justify-content-end mt-4">
                                                <Button 
                                                    variant="primary"
                                                    onClick={handleButtonClick}
                                                >
                                                    Continue to Photos Section
                                                    <FaArrowRight className="ms-2" />
                                                </Button>
                                            </div>
                                        </>
                                    ) : (
                                        <div className="alert alert-warning">
                                            <h5>No policy data available</h5>
                                            <p>Please upload an insurance document first to see your summary.</p>
                                        </div>
                                    )}
                                </Card.Body>
                            </Card>
                        </div>
                    </div>
                </div>
            </main>
            
            <footer className="bg-primary text-white p-3 text-center">
            </footer>
        </div>
    );
}