import {    
    Button, 
    Spinner
  } from "react-bootstrap";

function AddEntityBtn({ loading, disableBtns, closeModal}){
    return(
        <>
             <hr></hr>
            {(loading || disableBtns) && (
                <Button
                    type="button"
                    variant="danger"
                    className="float-end ms-2"
                    disabled
                >
                    Cancel
                </Button>
                )}
                {!loading && !disableBtns && (
                <Button
                    type="button"
                    variant="danger"
                    onClick={() => closeModal()}
                    className="float-end ms-2"
                >
                    Cancel
                </Button>
                )}
                {(loading || disableBtns) && (
                <Button variant="primary" className="float-end" disabled>
                    <Spinner
                    as="span"
                    animation="grow"
                    size="sm"
                    role="status"
                    aria-hidden="true"
                    />
                    Loading...
                </Button>
                )}
                {!loading && !disableBtns && (
                <Button type="submit" className="float-end">
                    Add
                </Button>
            )}   
        </>
    );
}

export default AddEntityBtn;