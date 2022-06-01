import { Table } from 'react-bootstrap';

function GenericUserTable({ users }){
    return(
        <>
            <Table responsive striped bordered>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Primary Phone</th>
                        <th>Role</th>
                        <th>Locked</th>
                        <th>Active</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        users.length != 0 &&
                        users.map((user) => {                    
                            return (
                                <tr key={user.id}>
                                    <td>{user.id}</td>
                                    <td>{user.name}</td>
                                    <td>{user.email}</td>
                                    <td>{user.primaryPhone}</td>
                                    <td>{user.role}</td>
                                    <td>{user.locked}</td>
                                    <td>{user.active}</td>
                                </tr>
                            );
                        })
                    }
                    {
                        users.length == 0 &&
                        <tr>
                            <td colSpan={7} style={{textAlign:"center"}}>No employee available. Please add an employee.</td>
                        </tr>
                    }
                </tbody>
            </Table>
        </>
    )
}

export default GenericUserTable;