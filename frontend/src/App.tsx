import { BrowserRouter, Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import AdminLogin from "./pages/auth/AdminLogin";
import AuthProvider from "./features/auth/AuthProvider";
import Unauthorized from "./pages/auth/Unauthorized";
import RoleBasedRoute from "./components/RoleBasedRoute";
import AdminPannel from "./pages/admin/AdminPannel";
import PropertyDetails from "./pages/admin/PropertyDetails";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="login" element={<Login />} />
          <Route path="register" element={<Register />} />
          <Route path="admin/login" element={<AdminLogin />} />
          <Route path="unauthorized" element={<Unauthorized />} />
          <Route
            path="admin"
            element={
              <RoleBasedRoute scopes="PhotographyCompany">
                <Home />
              </RoleBasedRoute>
            }
          >
            <Route index element={<AdminPannel />} />
            <Route
              path="listings/:listingCaseId"
              element={<PropertyDetails />}
            />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
