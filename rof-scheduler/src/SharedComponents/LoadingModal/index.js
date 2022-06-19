import { Modal, Spinner } from 'react-bootstrap';

function LoadingModal({show}){
    return(
        <>
            <Modal show={show} size="sm" centered>
                <div>
                    <Spinner animation="grow" size="lg" className="mt-3 ms-3 mb-3"/>
                    <Spinner animation="grow" size="lg" className="mt-3 ms-3 mb-3"/>
                    <Spinner animation="grow" size="lg" className="mt-3 ms-3 mb-3"/>
                    <Spinner animation="grow" size="lg" className="mt-3 ms-3 mb-3"/>
                    <Spinner animation="grow" size="lg" className="mt-3 ms-3 mb-3"/>
                    <Spinner animation="grow" size="lg" className="mt-3 ms-3 mb-3"/>
                </div>
                
            </Modal>
        </>
    )
}

export default LoadingModal;