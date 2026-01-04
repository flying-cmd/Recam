import { BrowserRouter, Route, Routes } from "react-router-dom";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import AuthProvider from "./features/auth/AuthProvider";
import Unauthorized from "./pages/auth/Unauthorized";
import RoleBasedRoute from "./components/RoleBasedRoute";
import PropertyDetails from "./pages/admin/PropertyDetails";
import AppLayout from "./pages/AppLayout";
import AgentPage from "./pages/admin/AgentPage";
import ListingCasePage from "./pages/admin/ListingCasePage";

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route element={<AppLayout />}>
            <Route path="login" element={<Login />} />
            <Route path="register" element={<Register />} />
            <Route path="unauthorized" element={<Unauthorized />} />

            {/* Protected routes (PhotographyCompany) */}
            <Route element={<RoleBasedRoute scopes="PhotographyCompany" />}>
              <Route path="dashboard" element={<ListingCasePage />} />
              <Route path="listings" element={<ListingCasePage />} />
              <Route path="agents" element={<AgentPage />} />
              <Route
                path="listings/:listingCaseId"
                element={<PropertyDetails />}
              />
            </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
