using System;

namespace StateManagement.Database;

public class FunctionResult
{
    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public FunctionResult()
    {
    }

    public FunctionResult(int errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    //public bool IsSuccess => ErrorCode == ErrorCodes.NO_ERROR;

    //public bool IsError => ErrorCode != ErrorCodes.NO_ERROR;

    public static FunctionResult BuildError(int errorCode, string errorMessage)
    {
        return new FunctionResult(errorCode, errorMessage);
    }

    public static T BuildError<T>(int errorCode, string errorMessage)
        where T : FunctionResult, new()
    {
        T result = new T();

        result.ErrorCode = errorCode;
        result.ErrorMessage = errorMessage;

        return result;
    }

    //public static FunctionResult BuildError(NeoGamesMSAException exception)
    //{
    //    return new FunctionResult(exception.ErrorCode, exception.Message);
    //}

    //public static T BuildError<T>(NeoGamesMSAException exception)
    //    where T : FunctionResult, new()
    //{
    //    T result = new T();

    //    result.ErrorCode = exception.ErrorCode;
    //    result.ErrorMessage = exception.Message;

    //    return result;
    //}

    //public static FunctionResult BuildSuccess(Action<FunctionResult> setter = null)
    //{
    //    var result = new FunctionResult(ErrorCodes.NO_ERROR, ErrorMessages.NO_ERROR);

    //    setter?.Invoke(result);

    //    return result;
    //}

    //public static T BuildSuccess<T>(Action<T> setter = null)
    //    where T : FunctionResult, new()
    //{
    //    T result = new T();

    //    result.ErrorCode = ErrorCodes.NO_ERROR;
    //    result.ErrorMessage = ErrorMessages.NO_ERROR;
    //    setter?.Invoke(result);

    //    return result;
    //}
}