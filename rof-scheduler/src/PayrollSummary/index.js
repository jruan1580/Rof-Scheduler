function PayrollSummary({setLoginState}){
    
    const search = (e) => {
        e.preventDefault();        
    };
    
    return(
        <>
        <h1>Payroll Summary</h1>
        <br />
        <Form onSubmit={search}>
                <Row>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>First Name:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="text"
                                name="firstName"
                                defaultValue= ""
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>Last Name:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="text"
                                name="lastName"
                                defaultValue= ""
                            />
                        </Col>
                    </Form.Group>
                </Row>
                <Row>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>Start Date:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="date"
                                name="startDate"
                                defaultValue= ""
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Col} lg={6}>
                        <Form.Label>End Date:</Form.Label>
                        <Col lg={12}>
                            <Form.Control
                                type="date"
                                name="endDate"
                                defaultValue= ""
                            />
                        </Col>
                    </Form.Group>
                </Row>
                <br />
                <Button type="submit" variant="primary" className="float-end ms-2">Search</Button>
            </Form>
        </>
    );
}

export default PayrollSummary;