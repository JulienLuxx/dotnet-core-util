using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Util
{
    /// <summary>
    /// CommonResponseResultDtoInterface
    /// </summary>
    public interface IResultDto
    {
        dynamic Data { get; }

        bool HasData { get; set; }

        string Message { get; set; }

        long State { get; set; }
    }

    public interface IResultDto<T> : IResultDto
    {
        new T Data { get; set; }
    }

    /// <summary>
    /// CommonResponseResultDtoStruct
    /// </summary>
    public struct ResultDto : IResultDto
    {
        public dynamic Data { get; set; }

        public bool HasData { get; set; }

        public string Message { get; set; }

        public long State { get; set; }

        public ResultDto(dynamic data, bool hasData, string message, int state)
        {
            Data = data;
            HasData = hasData;
            Message = message;
            State = state;
        }

        public ResultDto(dynamic data, bool hasData, string message, long state)
        {
            Data = data;
            HasData = hasData;
            Message = message;
            State = state;
        }

        //public ResultDto(dynamic data, int state = 200, string message = null)
        //{
        //    Data = data;
        //    HasData = null != data;
        //    Message = message;
        //    State = state;
        //}

        public ResultDto(dynamic data, long state = 200, string message = null)
        {
            Data = data;
            HasData = null != data;
            Message = message;
            State = state;
        }

        public ResultDto(string message, int state = -1)
        {
            Data = default;
            HasData = false;
            Message = message;
            State = state;
        }

        public ResultDto(int state, string message = null)
        {
            Data = default;
            HasData = false;
            Message = message;
            State = state;
        }

        public ResultDto(long state, string message = null)
        {
            Data = default;
            HasData = false;
            Message = message;
            State = state;
        }
    }

    public struct ResultDto<T> : IResultDto<T> 
    {
        public T Data { get; set; }

        dynamic IResultDto.Data { get => Data; }

        //public Type DataType { get; set; }

        public bool HasData { get; set; }

        public string Message { get; set; }

        public long State { get; set; }

        public ResultDto(T data, bool hasData, string message, int state)
        {
            Data = data;
            HasData = hasData;
            Message = message;
            State = state;
        }

        public ResultDto(T data, bool hasData, string message, long state)
        {
            Data = data;
            HasData = hasData;
            Message = message;
            State = state;
        }

        //public ResultDto(T data, int state = 200, string message = null) 
        //{
        //    Data = data;
        //    HasData = null != data;
        //    Message = message;
        //    State = state;
        //}

        public ResultDto(T data, long state = 200, string message = null)
        {
            Data = data;
            HasData = null != data;
            Message = message;
            State = state;
        }

        public ResultDto(string message, int state = -1) 
        {
            Data = default;
            HasData = false;
            Message = message;
            State = state;
        }

        public ResultDto(int state, string message = null)
        {
            Data = default;
            HasData = false;
            Message = message;
            State = state;
        }

        public ResultDto(long state, string message = null)
        {
            Data = default;
            HasData = false;
            Message = message;
            State = state;
        }
    }
}
