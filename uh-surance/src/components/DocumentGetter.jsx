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

    return (
        <div>
            <button onClick={handleClick}>
                Upload Document
            </button>
            <input type='file' onChange={handleFileChange} ref={fileInputRef}/>
        </div>
    )
}