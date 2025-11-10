package pe.edu.pucp.softpac.bo;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;
import pe.edu.pucp.softpac.bo.util.PasswordUtil;
import pe.edu.pucp.softpac.dao.UsuarioPaisAccesoDAO;
import pe.edu.pucp.softpac.dao.UsuariosDAO;
import pe.edu.pucp.softpac.daoImpl.UsuarioPaisAccesoDAOImpl;
import pe.edu.pucp.softpac.daoImpl.UsuariosDAOImpl;
import pe.edu.pucp.softpac.model.PaisesDTO;
import pe.edu.pucp.softpac.model.UsuarioPaisAccesoDTO;
import pe.edu.pucp.softpac.model.UsuariosDTO;

public class UsuariosBO {

    private UsuariosDAO usuariosDAO;
    private UsuarioPaisAccesoDAO usuariosPaisAccesoDAO;

    public UsuariosBO() {
        this.usuariosDAO = new UsuariosDAOImpl();
        this.usuariosPaisAccesoDAO = new UsuarioPaisAccesoDAOImpl();
    }

    public UsuariosDTO autenticarUsuarioPorNombreUsuario(String nombreUsuario, String password) {
        UsuariosDTO usuario = this.usuariosDAO.obtenerPorNombreDeUsuario(nombreUsuario.trim());

        if (usuario == null) {
            return new UsuariosDTO(); // Usuario no encontrado
        }

        Boolean esValido = PasswordUtil.checkPassword(password.trim(), usuario.getPassword_hash());

        if (esValido && usuario.getFecha_eliminacion() != null) {
            UsuariosDTO usuarioEliminado = new UsuariosDTO();
            usuarioEliminado.setUsuario_id(-1);
            return usuarioEliminado;
        }

        if (esValido && usuario.getFecha_eliminacion() == null) {
            for (UsuarioPaisAccesoDTO upa : usuario.getUsuario_pais()) {
                upa.setUsuario(null);
            }
            return usuario; // Autenticación exitosa
        }

        return new UsuariosDTO(); // Contraseña incorrecta
    }

    //VIEJO:
    public UsuariosDTO autenticarUsuarioPorCorreo(String correo_usuario, String password) {
        UsuariosDTO usuario = this.usuariosDAO.obtenerPorCorreo(correo_usuario);

        if (usuario == null) {
            return null; //Usuario no encontrado
        }

        Boolean esValido = PasswordUtil.checkPassword(password.trim(), usuario.getPassword_hash());

        if (esValido) {
            //PARA SERIALIZACIÓN:
            for (UsuarioPaisAccesoDTO upa : usuario.getUsuario_pais()) {
                upa.setUsuario(null);
            }
            return usuario; //Autenticación exitosa
        }

        return null; //Contraseña incorrecta

    }

    public ArrayList<UsuariosDTO> listarTodos() {
        return (ArrayList<UsuariosDTO>) usuariosDAO.listarTodos();
    }

//    public UsuariosDTO obtenerPorId(Integer usuario_id){
//        UsuariosDTO usuario = new UsuariosDTO();
//        usuario.setUsuario_id(usuario_id);
//        
//        return usuariosDAO.obtenerPorId(usuario_id);
//    }
    public UsuariosDTO obtenerPorId(Integer usuario_id) {
        UsuariosDTO usuario = new UsuariosDTO();
        usuario.setUsuario_id(usuario_id);

        usuario = usuariosDAO.obtenerPorId(usuario_id);

        //PARA SERIALIZACIÓN:
        for (UsuarioPaisAccesoDTO upa : usuario.getUsuario_pais()) {
            upa.setUsuario(null);
        }

        return usuario;
    }

    public Integer insertarUsuario(UsuariosDTO nuevoUsuario) {
        // ✅ VALIDACIÓN: Verificar que tenga al menos un país con acceso
        if (nuevoUsuario.getUsuario_pais() == null || nuevoUsuario.getUsuario_pais().isEmpty()) {
            try {
                throw new Exception("Debe seleccionar al menos un país permitido.");
            } catch (Exception ex) {
                Logger.getLogger(UsuariosBO.class.getName()).log(Level.SEVERE, null, ex);
                return 0;
            }
        }

        boolean tieneAlMenosUnPaisConAcceso = false;
        for (UsuarioPaisAccesoDTO acceso : nuevoUsuario.getUsuario_pais()) {
            if (acceso.getAcceso() != null && acceso.getAcceso()) {
                tieneAlMenosUnPaisConAcceso = true;
                break;
            }
        }

        if (!tieneAlMenosUnPaisConAcceso) {
            try {
                throw new Exception("Debe seleccionar al menos un país permitido.");
            } catch (Exception ex) {
                Logger.getLogger(UsuariosBO.class.getName()).log(Level.SEVERE, null, ex);
                return 0;
            }
        }

        // Validar contraseña
        if (nuevoUsuario.getPassword_hash() == null) {
            try {
                throw new Exception("La contraseña es obligatoria para nuevos usuarios.");
            } catch (Exception ex) {
                Logger.getLogger(UsuariosBO.class.getName()).log(Level.SEVERE, null, ex);
                return 0;
            }
        }

        String hashedPassword = PasswordUtil.hashPassword(nuevoUsuario.getPassword_hash());
        nuevoUsuario.setPassword_hash(hashedPassword);

        // ✅ Establecer la relación usuario para cada acceso
        for (UsuarioPaisAccesoDTO acceso : nuevoUsuario.getUsuario_pais()) {
            acceso.setUsuario(nuevoUsuario);
        }

        return usuariosDAO.insertar(nuevoUsuario);
    }

    public Integer modificarAccesoUsuario(int usuarioId, String nuevoNombreUsuario,
            Boolean activo, List<UsuarioPaisAccesoDTO> paisesAccesos, String nuevaPassword) {
        UsuariosDTO usuario = usuariosDAO.obtenerPorId(usuarioId);
        if (usuario == null) {
            return 0;
        }

        // ✅ VALIDACIÓN: Debe tener al menos un país con acceso = true
        boolean tieneAlMenosUnPaisConAcceso = false;
        for (UsuarioPaisAccesoDTO acceso : paisesAccesos) {
            if (acceso.getAcceso() != null && acceso.getAcceso()) {
                tieneAlMenosUnPaisConAcceso = true;
                break;
            }
        }

        if (!tieneAlMenosUnPaisConAcceso) {
            try {
                throw new Exception("Debe seleccionar al menos un país permitido.");
            } catch (Exception ex) {
                Logger.getLogger(UsuariosBO.class.getName()).log(Level.SEVERE, null, ex);
                return 0;
            }
        }

        usuario.setNombre_de_usuario(nuevoNombreUsuario);
        usuario.setActivo(activo);

        // ✅ Si se proporciona nueva contraseña, hashearla
        if (nuevaPassword != null && !nuevaPassword.trim().isEmpty()) {
            String hashedPassword = PasswordUtil.hashPassword(nuevaPassword.trim());
            usuario.setPassword_hash(hashedPassword);
        }

        // ✅ Actualizar la lista de accesos (ahora con TODOS los países)
        usuario.getUsuario_pais().clear();
        for (UsuarioPaisAccesoDTO acceso : paisesAccesos) {
            acceso.setUsuario(usuario);
            usuario.getUsuario_pais().add(acceso);
        }

        return usuariosDAO.modificar(usuario);
    }

    public Integer eliminarUsuario(UsuariosDTO usuario, UsuariosDTO usuarioActual) {
        UsuariosDTO usuarioCompleto = usuariosDAO.obtenerPorId(usuario.getUsuario_id());

        if (usuarioCompleto == null) {
            return 0;
        }

        usuarioCompleto.setFecha_eliminacion(new Date());
        usuarioCompleto.setUsuario_eliminacion(usuarioActual);

        usuarioCompleto.setActivo(false);

        return usuariosDAO.eliminarLogico(usuarioCompleto);
    }

}
