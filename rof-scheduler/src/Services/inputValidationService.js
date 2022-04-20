export const validateLoginPassword = function(password){
    var errMsg = '';
    
    if (password.length == 0){
        errMsg = 'Please enter a password.';
        return errMsg;
    }

    if (password.length < 8 || password.length > 32){
        errMsg = 'Ensure password length is between 8 and 32 characters.';
    }
    
    return errMsg;
}