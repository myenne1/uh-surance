import React, {useState, useRef} from 'react'
import Button from 'react-bootstrap/Button'
import { useNavigate } from 'react-router';



export default function DocumentGetter() {
    const navigate = useNavigate()
    const [file, setFile] = useState(null)
    const [fileUploaded, setFileUploaded] = useState(false)
    const fileInputRef = useRef(null);

    const handleFileChange = (event) => {
        if (event.target.files.length > 0) {
          setFileUploaded(true)
        }
        else {
          setFileUploaded(false)
        }
    }

    const handleClick = () => {
        fileInputRef.current.click()
    }

    const handleFileUpload = async (event) => {
        console.log("Sent file")
        const file = event.target.files[0];
        const formData = new FormData();
        formData.append('file', file);
      
        try {
          const response = await fetch('YOUR_UPLOAD_ENDPOINT', {
            method: 'POST',
            body: formData,
          });
      
          if (response.ok) {
            const data = await response.json();
            console.log('Upload successful:', data);
          } else {
            console.error('Upload failed:', response.statusText);
          }
        } catch (error) {
          console.error('Error during upload:', error);
        }
        navigate("/second-page")
    };

    return (
        <div>
            <input type='file' onChange={(e) => { handleFileChange(e)}} ref={fileInputRef} />
            {fileUploaded && (
                    <Button
                        variant="primary"
                        type="submit"
                        onClick={() => handleFileUpload({ target: { files: [fileInputRef.current.files[0]] }})}
                        className="d-block mx-auto mt-3"
                    >
                        Submit
                    </Button>
            )}
        </div>
    )
}