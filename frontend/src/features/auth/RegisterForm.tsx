import { useState } from "react";
import { useNavigate } from "react-router-dom";
import InputBox from "../../components/InputBox";
import { registerSchema } from "./schema";
import {
  MapZodErrorsToFields,
  type FieldErrors,
} from "../../utils/MapZodErrorsToFields";
import { register } from "../../services/authApi";
import type { IRegisterResponse } from "../../types/IAuth";
import type { IApiError } from "../../types/IApiResponse";
import PopupBox from "../../components/PopupBox";

type RegisterFormFieldsErrors =
  | "email"
  | "password"
  | "confirmPassword"
  | "firstName"
  | "lastName"
  | "companyName"
  | "avatarFile";

export default function RegisterForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [companyName, setCompanyName] = useState("");
  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const [errors, setErrors] = useState<FieldErrors<RegisterFormFieldsErrors>>(
    {}
  );
  const [errorPopup, setErrorPopup] = useState({ open: false, message: "" });
  const closeErrorPopup = () => setErrorPopup({ open: false, message: "" });
  const navigate = useNavigate();

  const resetForm = () => {
    setEmail("");
    setPassword("");
    setConfirmPassword("");
    setFirstName("");
    setLastName("");
    setCompanyName("");
    setAvatarFile(null);
    setErrors({});
  };

  const handleAvatarChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setAvatarFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const formResult = registerSchema.safeParse({
      email,
      password,
      confirmPassword,
      firstName,
      lastName,
      companyName,
      avatarFile,
    });

    // if the form is not valid
    if (!formResult.success) {
      setErrors(MapZodErrorsToFields(formResult.error));
      return;
    }

    // if the form is valid
    setErrors({});

    try {
      const res: IRegisterResponse = await register({
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
        companyName,
        avatarFile: avatarFile as File,
      });

      localStorage.setItem("token", res.data);

      resetForm();
      navigate("/home");
    } catch (error: unknown) {
      console.error(error);
      setErrorPopup({ open: true, message: (error as IApiError).title });
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded-lg shadow-lg max-w-md w-full">
        <h2 className="text-2xl font-bold mb-7 text-center text-gray-800">
          Register
        </h2>

        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <InputBox
              id="email"
              label="Email"
              type="email"
              value={email}
              setValue={setEmail}
              placeholder="Enter your email"
              error={errors.email}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="firstName"
              label="First Name"
              type="text"
              value={firstName}
              setValue={setFirstName}
              placeholder="Enter your first name"
              error={errors.firstName}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="lastName"
              label="Last Name"
              type="text"
              value={lastName}
              setValue={setLastName}
              placeholder="Enter your last name"
              error={errors.lastName}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="companyName"
              label="Company Name"
              type="text"
              value={companyName}
              setValue={setCompanyName}
              placeholder="Enter your company name"
              error={errors.companyName}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="password"
              label="Password"
              type="password"
              value={password}
              setValue={setPassword}
              placeholder="Enter your password"
              error={errors.password}
            />
          </div>

          <div className="mb-6">
            <InputBox
              id="confirmPassword"
              label="Confirm Password"
              type="password"
              value={confirmPassword}
              setValue={setConfirmPassword}
              placeholder="Confirm your password"
              error={errors.confirmPassword}
            />
          </div>

          <div className="mb-6">
            <label
              htmlFor="avatarFile"
              className="block text-gray-700 text-sm font-bold mb-2"
            >
              Avatar
            </label>
            <input
              id="avatarFile"
              type="file"
              accept="image/jpeg,image/png,image/webp"
              onChange={handleAvatarChange}
              className="block w-full text-sm text-gray-700 file:mr-4 file:rounded-md file:border-0 file:bg-gray-100 file:px-3 file:py-2 file:text-sm file:font-semibold hover:file:bg-gray-200"
            />
            {errors.avatarFile && (
              <p className="mt-1 text-sm text-red-600">{errors.avatarFile}</p>
            )}
          </div>

          <div className="flex items-center justify-between ">
            <button
              className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded cursor-pointer"
              type="submit"
            >
              Register
            </button>

            <button
              type="button"
              className="text-sm text-blue-500 hover:text-blue-700 cursor-pointer"
              onClick={() => navigate("/login")}
            >
              Back to Login
            </button>
          </div>
        </form>

        <PopupBox
          message={errorPopup.message}
          open={errorPopup.open}
          onClose={closeErrorPopup}
        />
      </div>
    </div>
  );
}
