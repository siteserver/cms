using System;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EUserActionType
    {
        Login,              //登录成功
        LoginFailed,              //登录失败
        WritingAdd,         //新增稿件
        WritingEdit,        //编辑稿件
        WritingDelete,      //删除稿件
        UpdateEmail,        //修改邮箱
        UpdateMobile,       //修改手机
        UpdatePassword,     //修改密码
    }

    public class EUserActionTypeUtils
    {
        public static string GetValue(EUserActionType type)
        {
            if (type == EUserActionType.Login)
            {
                return "Login";
            }
            if (type == EUserActionType.LoginFailed)
            {
                return "LoginFailed";
            }
            if (type == EUserActionType.WritingAdd)
            {
                return "WritingAdd";
            }
            if (type == EUserActionType.WritingEdit)
            {
                return "WritingEdit";
            }
            if (type == EUserActionType.WritingDelete)
            {
                return "WritingDelete";
            }
            if (type == EUserActionType.UpdateEmail)
            {
                return "UpdateEmail";
            }
            if (type == EUserActionType.UpdateMobile)
            {
                return "UpdateMobile";
            }
            if (type == EUserActionType.UpdatePassword)
            {
                return "UpdatePassword";
            }
            throw new Exception();
        }

        public static string GetText(EUserActionType type)
        {
            if (type == EUserActionType.Login)
            {
                return "用户登录成功";
            }
            if (type == EUserActionType.LoginFailed)
            {
                return "用户登录失败";
            }
            if (type == EUserActionType.WritingAdd)
            {
                return "新增稿件";
            }
            if (type == EUserActionType.WritingEdit)
            {
                return "编辑稿件";
            }
            if (type == EUserActionType.WritingDelete)
            {
                return "删除稿件";
            }
            if (type == EUserActionType.UpdateEmail)
            {
                return "修改邮箱";
            }
            if (type == EUserActionType.UpdateMobile)
            {
                return "修改手机";
            }
            if (type == EUserActionType.UpdatePassword)
            {
                return "修改密码";
            }
            throw new Exception();
        }

        public static EUserActionType GetEnumType(string typeStr)
        {
            if (Equals(typeStr, EUserActionType.Login))
                return EUserActionType.Login;
            if (Equals(typeStr, EUserActionType.LoginFailed))
                return EUserActionType.LoginFailed;
            if (Equals(typeStr, EUserActionType.WritingAdd))
                return EUserActionType.WritingAdd;
            if (Equals(typeStr, EUserActionType.WritingEdit))
                return EUserActionType.WritingEdit;
            if (Equals(typeStr, EUserActionType.WritingDelete))
                return EUserActionType.WritingDelete;
            if (Equals(typeStr, EUserActionType.UpdateEmail))
                return EUserActionType.UpdateEmail;
            if (Equals(typeStr, EUserActionType.UpdateMobile))
                return EUserActionType.UpdateMobile;
            if (Equals(typeStr, EUserActionType.UpdatePassword))
                return EUserActionType.UpdatePassword;
            throw new Exception();
        }

        public static bool Equals(string typeStr, EUserActionType type)
        {
            if (string.IsNullOrEmpty(typeStr))
                return false;
            if (string.Equals(typeStr.ToLower(), GetValue(type).ToLower()))
                return true;
            return false;
        }

        public static bool Equals(EUserActionType type, string typeStr)
        {
            return Equals(typeStr, type);
        }
    }
}
