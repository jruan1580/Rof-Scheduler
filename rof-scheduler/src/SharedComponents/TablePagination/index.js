import { Pagination } from "react-bootstrap";

function TablePagination({ currPage, totalPages, setCurrPage }){
    return(
        <>
            <Pagination>
                {currPage != 1 && (
                    <Pagination.Prev onClick={() => setCurrPage(currPage - 1)} />
                )}
                {currPage != totalPages && totalPages !== 0 && (
                    <Pagination.Next onClick={() => setCurrPage(currPage + 1)} />
                )}
            </Pagination>
        </>
    );
}

export default TablePagination;