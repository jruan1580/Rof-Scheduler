import {    
    Button, 
    Spinner
} from "react-bootstrap";

function UpdateEntityBtn({ updating, closeModal }){
    return(
        <>
            <hr></hr>
            {updating && (
                <Button
                    type="button"
                    variant="danger"
                    className="float-end ms-2"
                    disabled
                >
                    Close
                </Button>
            )}
            {!updating && (
                <Button
                    type="button"
                    variant="danger"
                    onClick={() => closeModal()}
                    className="float-end ms-2"
                >
                    Close
                </Button>
            )}
            {updating && (
                <Button variant="primary" className="float-end" disabled>
                    <Spinner
                    as="span"
                    animation="grow"
                    size="sm"
                    role="status"
                    aria-hidden="true"
                    />
                    Updating...
                </Button>
            )}
            {!updating && (
                <Button type="submit" className="float-end">
                    Update
                </Button>
            )}
        </>
    );
}

export default UpdateEntityBtn;