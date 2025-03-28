import React, { useRef } from 'react';  // Add useRef import
import 'react-bootstrap';
import "bootstrap/dist/css/bootstrap.min.css";
import '../styles/WelcomePage.css';
import ValuablesGetter from '../components/ValuablesGetter';
import { useLocation, useNavigate } from 'react-router';
import { Button, Card, Badge, ListGroup } from 'react-bootstrap';
import { FaGem, FaCamera } from 'react-icons/fa';

export default function PersonalItemsPage() {
    const location = useLocation();
    const navigate = useNavigate();
    const uploadedData = location.state?.receivedData;
    
    // Create refs for each file input
    const fileInputRefs = useRef([]);
    
    // Initialize the refs array
    React.useEffect(() => {
        fileInputRefs.current = fileInputRefs.current.slice(0, 4);
    }, []);

    // Format currency values
    const formatCurrency = (value) => {
        if (!value && value !== 0) return 'N/A';
        // Remove non-numeric characters except decimal point
        const numericValue = value.toString().replace(/[^\d.]/g, '');
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            maximumFractionDigits: 0
        }).format(numericValue);
    };

    // Handle file input change
    const handleFileChange = (event, itemName) => {
        const file = event.target.files[0];
        if (file) {
            console.log(`File selected for ${itemName}:`, file.name);
            // You can add code here to handle the file
        }
    };

    // Define the items we want to display
    const personalItems = [
        {
            name: 'Jewelry and Furs',
            value: uploadedData?.['Jewelry and Furs'] || 'N/A',
            icon: <FaGem className="text-warning" />
        },
        {
            name: 'Silver and Gold Items',
            value: uploadedData?.['Silver Goldware Theft'] || 'N/A',
            icon: <FaGem className="text-secondary" />
        },
        {
            name: 'Business Property',
            value: uploadedData?.['Business Property'] || 'N/A',
            icon: <FaGem className="text-info" />
        },
        {
            name: 'Firearms',
            value: uploadedData?.['Firearms'] || 'N/A',
            icon: <FaGem className="text-danger" />
        }
    ];

    return (
        <div className="min-vh-100 d-flex flex-column">
            <header className="bg-primary text-white p-4 text-center fw-bold fs-4">
                Uh-Surance
            </header>
            
            <main className="flex-grow-1 d-flex align-items-start justify-content-center flex-column" style={{ paddingTop: "5vh" }}>
                <div className="container text-center">
                    <div className="row justify-content-center mb-4">
                        <section className="col-md-8 bg-light p-4 rounded shadow">
                            <h2 className="fw-semibold">Valuable Items Covered in Your Policy</h2>
                            <p className="text-muted">Your policy includes coverage for these valuable items</p>
                        </section>
                    </div>
                    
                    <div className="row justify-content-center mb-4">
                        <section className="col-md-8">
                            <Card className="shadow border-0">
                                <Card.Header className="bg-primary text-white">
                                    <h4 className="mb-0">Coverage Details</h4>
                                </Card.Header>
                                <ListGroup variant="flush">
                                    {personalItems.map((item, index) => (
                                        <ListGroup.Item key={index} className="py-4 border-bottom">
                                            <div className="d-flex justify-content-between align-items-center">
                                                <div className="d-flex align-items-center">
                                                    <div className="me-3 fs-3">
                                                        {item.icon}
                                                    </div>
                                                    <div className="text-start">
                                                        <h5 className="mb-1">{item.name}</h5>
                                                        <Badge bg="success" className="px-3 py-2">
                                                            Coverage: {formatCurrency(item.value)}
                                                        </Badge>
                                                    </div>
                                                </div>
                                                {/* Hidden file input */}
                                                <input
                                                    type="file"
                                                    accept="image/*"
                                                    style={{ display: 'none' }}
                                                    ref={el => fileInputRefs.current[index] = el}
                                                    onChange={(e) => handleFileChange(e, item.name)}
                                                />
                                                {/* Button that triggers the file input */}
                                                <Button 
                                                    variant="outline-primary" 
                                                    onClick={() => fileInputRefs.current[index].click()}
                                                >
                                                    <FaCamera className="me-2" /> Add Photo
                                                </Button>
                                            </div>
                                        </ListGroup.Item>
                                    ))}
                                </ListGroup>
                                <Card.Footer className="bg-light text-center p-3">
                                    <p className="mb-0 text-muted">
                                        Policy Type: {uploadedData?.['Type'] || 'Standard Policy'} | 
                                        Deductible: ${uploadedData?.['Deductible']?.replace(/[^\d]/g, '') || '500'}
                                    </p>
                                </Card.Footer>
                            </Card>
                        </section>
                    </div>
                </div>
                
                <div className="container text-center mt-2 mb-4">
                    <div className="row justify-content-center">
                        <div className="col-md-8 bg-light p-4 rounded shadow">
                            <h4 className="mb-3">Add Photos of Your Valuable Items</h4>
                            <p className="text-muted mb-4">Upload photos to help verify your items in case of a claim</p>
                            <ValuablesGetter/>
                        </div>
                    </div>
                </div>
            </main>
            
            <footer className="bg-primary text-white p-3 text-center">
            </footer>
        </div>
    );
}