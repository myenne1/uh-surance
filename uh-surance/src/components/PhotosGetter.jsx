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
        console.log("Sending files to backend");
        const postFormData = new FormData();
        
        files.forEach((file, index) => {
            postFormData.append(`file${index}`, file);
        });
        
        try {
            const response = await fetch('YOUR_UPLOAD_ENDPOINT', {
                method: 'POST',
                body: postFormData,
            });
      
            if (response.ok) {
                const data = await response.json();
                console.log('Upload successful:', data);
                navigate("/personal-items-page", { 
                    state: { 
                        uploadedData: data 
                    }
                });
            } else {
                console.error('Upload failed:', response.statusText);
            }
        } catch (error) {
            console.error('Error during upload:', error);
        }
        navigate("/personal-items-page")
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