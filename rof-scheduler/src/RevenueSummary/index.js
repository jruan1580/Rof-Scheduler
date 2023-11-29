import { Row, Form, Col, Button, Spinner, Alert } from "react-bootstrap";

function RevenueSummary({setLoginState}){
    return(
        <>
            <h1>REVENUE SUMMARY</h1>
            <br />
            <Row>
                <Form.Group as={Col} md='6'>
                    <Form.Label>Start Date:</Form.Label>
                    <Col lg={9}>
                        <Form.Control
                            type="date"
                            name="startDate"
                        />
                        {/* <Form.Control.Feedback type="invalid">
                            {validationMap.get("date")}
                        </Form.Control.Feedback> */}
                    </Col>
                </Form.Group>
                <Form.Group as={Col} md='6'>
                    <Form.Label>End Date:</Form.Label>
                    <Col lg={9}>
                        <Form.Control
                            type="date"
                            name="endDate"
                        />
                        {/* <Form.Control.Feedback type="invalid">
                            {validationMap.get("date")}
                        </Form.Control.Feedback> */}
                    </Col>
                </Form.Group>
            </Row>
            <br />
            <Button type="submit" variant="primary" className="float-end ms-2">Search</Button>

        </>
    );
}

export default RevenueSummary;