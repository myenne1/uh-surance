import React, {useState, useRef} from 'react'

export default function DocumentGetter() {
    const [file, setFile] = useState(null)
    const fileInputRef = useRef(null);

    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    }

    const handleClick = () => {
        fileInputRef.current.click()
    }
    async function uploadDatasetFile(formData) {
        try {
          const response = await axios.post(
            "http://localhost:5000/files/upload",
            formData,
            {headers: {'Content-Type': 'multipart/form-data'}}
          );
          return response.data;
        } catch (error: any) {
          throw error.response.data;
        }
      }

    return (
        <div>
            <button onClick={handleClick}>
                Upload Document
            </button>
            <input type='file' onChange={handleFileChange} ref={fileInputRef}/>
        </div>
    )
}