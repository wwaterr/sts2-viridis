"""Draw textured Spine triangles for the Viridis authoring previews."""
import numpy as np
from numba import njit


@njit
def raster(canvas,texture,points,uv,triangles):
    height,width=canvas.shape[:2]
    th,tw=texture.shape[:2]
    for a,b,c in triangles:
        ax,ay=points[a]; bx,by=points[b]; cx,cy=points[c]
        dx,dy=bx-ax,by-ay; ex,ey=cx-ax,cy-ay
        det=dx*ey-dy*ex
        if abs(det)<.0001: continue
        for y in range(max(0,int(np.floor(min(ay,by,cy)))),min(height,int(np.ceil(max(ay,by,cy))))):
            for x in range(max(0,int(np.floor(min(ax,bx,cx)))),min(width,int(np.ceil(max(ax,bx,cx))))):
                px=x+.5-ax; py=y+.5-ay
                s=(px*ey-py*ex)/det; t=(dx*py-dy*px)/det
                if min(s,t,1-s-t)<-1e-7: continue
                u=(uv[a,0]*(1-s-t)+uv[b,0]*s+uv[c,0]*t)*tw-.5
                v=(uv[a,1]*(1-s-t)+uv[b,1]*s+uv[c,1]*t)*th-.5
                xx=int(np.floor(u)); yy=int(np.floor(v)); fx=u-xx; fy=v-yy
                col=np.zeros(4)
                for j in range(2):
                    for i in range(2):
                        p=texture[min(max(yy+j,0),th-1),min(max(xx+i,0),tw-1)]
                        w=(fx if i else 1-fx)*(fy if j else 1-fy)*p[3]/255
                        col[:3]+=p[:3]*w;col[3]+=w
                for k in range(3):
                    canvas[y,x,k]=round(col[k]+canvas[y,x,k]*(1-col[3]))
