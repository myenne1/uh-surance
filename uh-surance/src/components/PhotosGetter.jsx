import React, { useState, useRef } from 'react';
import Button from 'react-bootstrap/Button';
import { useNavigate } from 'react-router';

export default function PhotosGetter() {
    const navigate = useNavigate();
    const [files, setFiles] = useState([]);
    const [fileUploaded, setFileUploaded] = useState(false);
    const fileInputRef = useRef(null);

    const handleFileChange = (event) => {
        const selectedFiles = Array.from(event.target.files);
        
        if (selectedFiles.length > 0 && selectedFiles.length <= 5) {
            setFiles(selectedFiles);
            setFileUploaded(true);
        } else {
            setFiles([]);
            setFileUploaded(false);
            if (selectedFiles.length > 5) {
                alert('Please select up to 5 files only');
            }
        }
    };

    const handleClick = () => {
        fileInputRef.current.click();
    };

    const handleFileUpload = async () => {
        const response = await fetch('/api/policies/insurance-fields/2066f1b3-c096-4d18-84e2-9a8a8365ac76', {
            method: 'GET',
        });
  
        if (response.ok) {
            const data2 = await response.json();
            console.log('Upload successful:', data2);
            navigate("/personal-items-page", { 
                state: { 
                    receivedData: data2
                }
            });
        } else {
            console.error('Upload failed:', response.statusText);
        }
    };

    return (
        <div>
            <input 
                type='file' 
                multiple 
                accept="image/*"
                onChange={(e) => handleFileChange(e)} 
                ref={fileInputRef}
            />
            
            {fileUploaded && (
                <div>
                    <Button
                        variant="primary"
                        type="submit"
                        onClick={handleFileUpload}
                        className="d-block mx-auto mt-3"
                    >
                        Submit
                    </Button>
                </div>
            )}
        </div>
    );
}